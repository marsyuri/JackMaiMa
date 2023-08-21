using JackMaiMa.Models;
using JackMaiMa.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace JackMaiMa.Controllers
{
    public class Order_DetailsController : Controller
    {
        private ApplicationDbContext _context;

        public Order_DetailsController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Create(int? orderId)
        {
            if (orderId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = _context.Orders.SingleOrDefault(o => o.Id == orderId && o.OrderStatusId == OrderStatus.Processing);
            if (order == null)
            {
                return HttpNotFound();
            }
            Order_Detail newDetail = new Order_Detail
            {
                OrderId = orderId.Value
            };
            Order_Detail_Create_ViewModel viewModel = new Order_Detail_Create_ViewModel
            {
                Order_Detail = newDetail,
                Orders = _context.Orders.Where(o => o.Id == orderId.Value).ToList(),
                Products = _context.Products.Where(p => !p.IsDeleted)
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Order_Detail detail)
        {
            if (!ModelState.IsValid)
            {
                Order_Detail_Create_ViewModel viewModel = new Order_Detail_Create_ViewModel()
                {
                    Order_Detail = detail,
                    Orders = _context.Orders.Where(o => o.Id == detail.OrderId).ToList(),
                    Products = _context.Products.Where(p => !p.IsDeleted)
                };
            }
            detail.CreateDate = DateTime.Now;
            detail.ModifiedDate = DateTime.Now;
            _context.Order_Details.Add(detail);
            _context.SaveChanges();
            return RedirectToAction("Calculate", new { id = detail.Id });
        }

        public ActionResult Calculate(int? detailId)
        {
            if (detailId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order_Detail detailInDb = _context.Order_Details.SingleOrDefault(d => d.Id == detailId);
            if (detailInDb == null)
            {
                return HttpNotFound();
            }
            decimal unitPrice = _context.Products.Where(p => p.Id == detailInDb.ProductId).Select(p => p.UnitPrice).Single();
            Order_Detail calDetail = new Order_Detail()
            {
                Id = detailId.Value,
                OrderId = detailInDb.OrderId,
                ProductId = detailInDb.ProductId,
                UnitPrice = unitPrice,
                Quantity = detailInDb.Quantity,
                TotalPricePerDetail = unitPrice * detailInDb.Quantity,
                CreatedBy = detailInDb.CreatedBy,
                CreateDate = detailInDb.CreateDate,
                ModifiedBy = detailInDb.ModifiedBy,
                ModifiedDate = detailInDb.ModifiedDate
            };
            Order_Detail_Create_ViewModel viewModel = new Order_Detail_Create_ViewModel
            {
                Order_Detail = calDetail,
                Orders = _context.Orders.Where(o => o.Id == detailInDb.OrderId).ToList(),
                Products = _context.Products.Where(p => p.Id == detailInDb.ProductId).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Calculate(Order_Detail detail)
        {
            if (!ModelState.IsValid)
            {
                Order_Detail_Create_ViewModel viewModel = new Order_Detail_Create_ViewModel()
                {
                    Order_Detail = detail,
                    Orders = _context.Orders.Where(o => o.Id == detail.OrderId).ToList(),
                    Products = _context.Products.Where(p => !p.IsDeleted).ToList()
                };
            }

            Product prodInDb = _context.Products.Single(p => p.Id == detail.ProductId);
            if (prodInDb.NumberInStock < detail.Quantity)
            {
                ModelState.AddModelError("Order_Detail.Quantity", "จำนวนที่สั่งมากกว่าจำนวนของใน Stock");
                Order_Detail_Create_ViewModel viewModel = new Order_Detail_Create_ViewModel()
                {
                    Order_Detail = detail,
                    Orders = _context.Orders.Where(o => o.Id == detail.OrderId).ToList(),
                    Products = _context.Products.Where(p => !p.IsDeleted).ToList()
                };
            }
            int oldStock = prodInDb.NumberInStock;
            prodInDb.NumberInStock -= detail.Quantity;
            prodInDb.ModifiedBy = detail.ModifiedBy;
            prodInDb.ModifiedDate = detail.ModifiedDate;

            int runNo = _context.StockLogs.Where(s => s.ProductId == prodInDb.Id).Max(s => s.RunningNo);
            runNo++;
            StockLog log = new StockLog()
            {
                RunningNo = runNo,
                LogNo = String.Format("PD{0}-{1}", prodInDb.Id, runNo),
                ProductId = prodInDb.Id,
                StockLogTypeId = StockLogType.Edit,
                OldStock = oldStock,
                NumberOfChange = detail.Quantity,
                CurrentStock = prodInDb.NumberInStock,
                Remarks = "ขายสินค้า",
                LogBy = detail.ModifiedBy,
                LogDate = DateTime.Now
            };
            _context.StockLogs.Add(log);

            Order orderInDb = _context.Orders.Single(o => o.Id == detail.OrderId);
            orderInDb.AllTotalPrice += detail.TotalPricePerDetail;
            orderInDb.DiscountPerOrder += detail.DiscountPerDetail;
            orderInDb.NetPrice = orderInDb.AllTotalPrice - orderInDb.DiscountPerOrder;
            orderInDb.ModifiedBy = detail.ModifiedBy;
            orderInDb.ModifiedDate = detail.ModifiedDate;

            Order_Detail detailInDb = _context.Order_Details.SingleOrDefault(d => d.Id == detail.Id);
            detailInDb.DiscountPerDetail = detail.DiscountPerDetail;

            _context.SaveChanges();
            return RedirectToAction("Details", "Orders", new { id = detail.OrderId });
        }
    }
}