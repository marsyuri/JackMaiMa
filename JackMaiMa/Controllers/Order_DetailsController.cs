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
            Order_Detail_Create_ViewModel viewModel = new Order_Detail_Create_ViewModel
            {
                OrderId = orderId.Value,
                Orders = _context.Orders.Where(o => o.Id == orderId.Value).ToList(),
                Products = _context.Products.Where(p => !p.IsDeleted)
            };
            return View("Order_DetailForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Order_Detail detail)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detail.CreateDate = DateTime.Now;
            detail.ModifiedDate = DateTime.Now;
           
            Product prodInDb = _context.Products.Single(p => p.Id == detail.ProductId);
            if (prodInDb.NumberInStock < detail.Quantity)
            {
                ModelState.AddModelError("Order_Detail.Quantity", "จำนวนที่สั่งมากกว่าจำนวนของใน Stock");
            }
            int oldStock = prodInDb.NumberInStock;
            prodInDb.NumberInStock -= detail.Quantity;
            prodInDb.ModifiedBy = detail.ModifiedBy;
            prodInDb.ModifiedDate = detail.ModifiedDate;

            detail.UnitPrice = prodInDb.UnitPrice;
            detail.TotalPricePerDetail = detail.Quantity * detail.UnitPrice;
            _context.Order_Details.Add(detail);

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

            _context.SaveChanges();

            return RedirectToAction("Details", "Orders", new { id = detail.OrderId });
        }
    }
}