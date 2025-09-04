using JackMaiMa.Models;
using JackMaiMa.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace JackMaiMa.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private ApplicationDbContext _context;

        public OrdersController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Orders
        public ActionResult Index(string sortOrder, string currentFilter, string searchBox, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SortByOrderNo = sortOrder == "order_no" ? "order_no_desc" : "order_no";
            ViewBag.SortByCustName = sortOrder == "cust_name" ? "cust_name_desc" : "cust_name";
            ViewBag.SortByOrderDate = sortOrder == "order_date" ? "order_date_desc" : "order_date";
            ViewBag.SortByRequiredDate = sortOrder == "required_date" ? "required_date_desc" : "required_date";
            ViewBag.SortByTotalPrice = sortOrder == "total_price" ? "total_price_desc" : "total_price";
            ViewBag.SortByDiscount = sortOrder == "discount" ? "discount_desc" : "discount";
            ViewBag.SortByNetPrice = sortOrder == "net_price" ? "net_price_desc" : "net_price";
            ViewBag.SortByCreatedBy = sortOrder == "created_by" ? "created_by_desc" : "created_by";
            ViewBag.SortByCreateDate = sortOrder == "create_date" ? "create_date_desc" : "create_date";
            ViewBag.SortByStatus = sortOrder == "status" ? "status_desc" : "status";

            if (searchBox != null)
            {
                page = 1;
            }
            else
            {
                searchBox = currentFilter;
            }
            ViewBag.CurrentFilter = searchBox;
            ViewBag.Search = searchBox;
            IQueryable<Order> orders = _context.Orders.Include(o => o.Customer)
                                            .Include(o => o.OrderStatus)
                                            .Select(o => o);
            if (!string.IsNullOrWhiteSpace(searchBox))
            {
                CultureInfo culture = CultureInfo.CreateSpecificCulture("nl-BE");
                DateTimeStyles styles = DateTimeStyles.None;
                DateTime searchDate;
                if (DateTime.TryParse(searchBox, culture, styles, out searchDate))
                {
                    searchDate = searchDate.Date;
                    orders = orders.Where(o => DbFunctions.TruncateTime(o.OrderDate) == searchDate
                                    || DbFunctions.TruncateTime(o.RequiredDate) == searchDate
                                    || DbFunctions.TruncateTime(o.CreateDate) == searchDate);
                }
                else
                {
                    searchBox = searchBox.ToLower();
                    orders = orders.Where(o => o.OrderNo.ToLower().Contains(searchBox)
                                    || o.Customer.Name.ToLower().Contains(searchBox)
                                    || o.AllTotalPrice.ToString().Contains(searchBox)
                                    || o.DiscountPerOrder.ToString().Contains(searchBox)
                                    || o.NetPrice.ToString().Contains(searchBox)
                                    || o.CreatedBy.ToLower().Contains(searchBox));
                }
            }
            if (orders.Count() > 0)
            {
                orders = SelectOrder(sortOrder, orders);
            }
            int pageSize = GlobalValue.LargePageSize;
            int pageNumber = (page ?? 1);
            return View(orders.ToPagedList(pageNumber, pageSize));
        }

        private IQueryable<Order> SelectOrder(string sortOrder, IQueryable<Order> orders)
        {
            switch (sortOrder)
            {
                case "order_no":
                    orders = orders.OrderBy(o => o.OrderNo);
                    break;
                case "order_no_desc":
                    orders = orders.OrderByDescending(o => o.OrderNo);
                    break;
                case "cust_name":
                    orders = orders.OrderBy(o => o.Customer.Name);
                    break;
                case "cust_name_desc":
                    orders = orders.OrderByDescending(o => o.Customer.Name);
                    break;
                case "order_date":
                    orders = orders.OrderBy(o => o.OrderDate);
                    break;
                case "order_date_desc":
                    orders = orders.OrderByDescending(o => o.OrderDate);
                    break;
                case "required_date":
                    orders = orders.OrderBy(o => o.RequiredDate);
                    break;
                case "required_date_desc":
                    orders = orders.OrderByDescending(o => o.RequiredDate);
                    break;
                case "total_price":
                    orders = orders.OrderBy(o => o.AllTotalPrice);
                    break;
                case "total_price_desc":
                    orders = orders.OrderByDescending(o => o.AllTotalPrice);
                    break;
                case "discount":
                    orders = orders.OrderBy(o => o.DiscountPerOrder);
                    break;
                case "discount_desc":
                    orders = orders.OrderByDescending(o => o.DiscountPerOrder);
                    break;
                case "net_price":
                    orders = orders.OrderBy(o => o.NetPrice);
                    break;
                case "net_price_desc":
                    orders = orders.OrderByDescending(o => o.NetPrice);
                    break;
                case "created_by":
                    orders = orders.OrderBy(o => o.CreatedBy);
                    break;
                case "created_by_desc":
                    orders = orders.OrderByDescending(o => o.CreatedBy);
                    break;
                case "create_date":
                    orders = orders.OrderBy(o => o.CreateDate);
                    break;
                case "create_date_desc":
                    orders = orders.OrderByDescending(o => o.CreateDate);
                    break;
                case "status":
                    orders = orders.OrderBy(o => o.OrderStatus.Name);
                    break;
                case "status_desc":
                    orders = orders.OrderByDescending(o => o.OrderStatus.Name);
                    break;
                default:
                    orders = orders.OrderByDescending(o => o.Id);
                    break;
            }
            return orders;
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = _context.Orders.Include(o => o.Customer)
                                .Include(o => o.OrderStatus)
                                .SingleOrDefault(o => o.Id == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            List<Order_Detail> order_details = _context.Order_Details.Include(d => d.Product)
                                                        .Where(d => d.OrderId == id)
                                                        .ToList();
            OrderDetail_ViewModel viewModel = new OrderDetail_ViewModel
            {
                Order = order,
                Order_Details = order_details
            };
            return View(viewModel);
        }

        // GET: Orders/Create
        public ActionResult Create(int? customerId)
        {
            if (customerId == null)
            {
                ViewBag.IsCanChangeCustId = true;
                customerId = 0;
            }
            else
            {
                Customer customer = _context.Customers.SingleOrDefault(c => c.Id == customerId);
                if (customer == null)
                {
                    return HttpNotFound();
                }
            }
            int runNo = CreateRunNo();
            Order newOrder = new Order()
            {
                RunNo = runNo,
                OrderNo = CreateOrderNo(runNo),
                OrderDate = DateTime.Now,
                RequiredDate = DateTime.Now,
                CustomerId = customerId.Value,
                OrderStatusId = OrderStatus.Processing
            };
            OrderFormViewModel viewModel = new OrderFormViewModel()
            {
                Order = newOrder,
                Customers = _context.Customers.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToList()
            };
            return View("OrderForm", viewModel);
        }

        private string CreateOrderNo(int runNo)
        {
            string runNoText = runNo.ToString().PadLeft(4, '0');
            return String.Format("J{0}-{1}", DateTime.Now.ToString("yyyyMMdd"), runNoText);
        }

        private int CreateRunNo()
        {
            int runNo = 0;
            if (_context.Orders.Count() > 0)
            {
                DateTime yesterday = DateTime.Now.AddDays(-1.0).Date;
                IQueryable<Order> todayOrder = _context.Orders.Where(o => DbFunctions.TruncateTime(o.CreateDate) > yesterday).Select(o => o);
                if (todayOrder.Count() > 0)
                {
                    runNo = todayOrder.Max(o => o.RunNo);
                }
            }
            runNo++;
            return runNo;
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = _context.Orders.SingleOrDefault(o => o.Id == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            OrderFormViewModel viewModel = new OrderFormViewModel()
            {
                Order = order,
                Customers = _context.Customers.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToList()
            };
            return View("OrderForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Order order, bool isCanChangeCustId)
        {
            if (!ModelState.IsValid)
            {
                if (isCanChangeCustId)
                {
                    ViewBag.IsCanChangeCustId = true;
                }
                OrderFormViewModel viewModel = new OrderFormViewModel
                {
                    Order = order,
                    Customers = _context.Customers.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToList()
                };
                return View("OrderForm", viewModel);
            }
            if (order.Id == 0)
            {
                order.CreateDate = DateTime.Now;
                order.ModifiedDate = DateTime.Now;
                _context.Orders.Add(order);
            }
            else
            {
                Order orderInDb = _context.Orders.Single(o => o.Id == order.Id);
                orderInDb.OrderDate = order.OrderDate;
                orderInDb.RequiredDate = order.RequiredDate;
                orderInDb.AllTotalPrice = order.AllTotalPrice;
                orderInDb.DiscountPerOrder = order.DiscountPerOrder;
                orderInDb.NetPrice = order.NetPrice;
                orderInDb.ModifiedBy = order.ModifiedBy;
                orderInDb.ModifiedDate = DateTime.Now;
            }
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = order.Id });
        }

        public ActionResult Close(int? id, byte? orderStatus)
        {
            if (id == null || orderStatus == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = _context.Orders.Include(o => o.Customer).SingleOrDefault(o => o.Id == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            if (orderStatus != OrderStatus.Canceled && orderStatus != OrderStatus.Finished)
            {
                return HttpNotFound();
            }
            ViewBag.OrderStatus = orderStatus;
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Close(int id, byte orderStatus)
        {
            string currentUser = GetCurrentUser();
            Order orderInDb = _context.Orders.Single(o => o.Id == id);
            orderInDb.OrderStatusId = orderStatus;
            if (orderStatus == OrderStatus.Canceled)
            {
                var order_details = _context.Order_Details.Where(od => od.OrderId == id);
                foreach (var item in order_details)
                {
                    var prodInDb = _context.Products.SingleOrDefault(p => p.Id == item.ProductId);
                    if (prodInDb != null)
                    {
                        int oldStock = prodInDb.NumberInStock;
                        prodInDb.NumberInStock += item.Quantity;
                        prodInDb.ModifiedBy = currentUser;
                        prodInDb.ModifiedDate = DateTime.Now;

                        int runNo = _context.StockLogs.Where(s => s.ProductId == prodInDb.Id).Max(s => s.RunningNo);
                        runNo++;
                        StockLog log = new StockLog()
                        {
                            RunningNo = runNo,
                            LogNo = String.Format("PD{0}-{1}", prodInDb.Id, runNo),
                            ProductId = prodInDb.Id,
                            StockLogTypeId = StockLogType.CancelOrder,
                            OldStock = oldStock,
                            NumberOfChange = item.Quantity,
                            CurrentStock = prodInDb.NumberInStock,
                            Remarks = "Cancel Order",
                            LogBy = item.ModifiedBy,
                            LogDate = DateTime.Now
                        };
                        _context.StockLogs.Add(log);
                    }
                }
            }
            orderInDb.ModifiedBy = currentUser;
            orderInDb.ModifiedDate = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = orderInDb.Id });
        }

        private string GetCurrentUser()
        {
            string username = User.Identity.Name;
            if (!String.IsNullOrWhiteSpace(username))
            {
                int assignIndex = username.IndexOf('@');
                username = username.Substring(0, assignIndex);
            }
            return username;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
