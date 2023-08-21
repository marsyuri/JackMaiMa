using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JackMaiMa.Models;
using JackMaiMa.ViewModels;
using System.Globalization;
using PagedList;

namespace JackMaiMa.Controllers
{
    [Authorize]
    public class StockLogsController : Controller
    {
        private ApplicationDbContext _context;

        public StockLogsController()
        {
            _context = new ApplicationDbContext();
        }

        [Route("stocklogs/product/{productId}")]
        public ActionResult Index(int productId, string sortOrder, string currentFilter, string searchBox, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SortByLogNo = sortOrder == "log_no" ? "log_no_desc" : "log_no";
            ViewBag.SortByOldStock = sortOrder == "old_stock" ? "old_stock_desc" : "old_stock";
            ViewBag.SortByNoOfChange = sortOrder == "no_of_change" ? "no_of_change_desc" : "no_of_change";
            ViewBag.SortByCurStock = sortOrder == "cur_stock" ? "cur_stock_desc" : "cur_stock";
            ViewBag.SortByLogBy = sortOrder == "log_by" ? "log_by_desc" : "log_by";
            ViewBag.SortByLogDate = sortOrder == "log_date" ? "log_date_desc" : "log_date";
            ViewBag.SortByLogType = sortOrder == "log_type" ? "log_type_desc" : "log_type";
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
            IQueryable<StockLog> stockLogs = _context.StockLogs.Include(s => s.StockLogType).Where(s => s.ProductId == productId);
            if (!string.IsNullOrWhiteSpace(searchBox))
            {
                CultureInfo culture = CultureInfo.CreateSpecificCulture("nl-BE");
                DateTimeStyles styles = DateTimeStyles.None;
                DateTime searchDate;
                if (DateTime.TryParse(searchBox, culture, styles, out searchDate))
                {
                    searchDate = searchDate.Date;
                    stockLogs = stockLogs.Where(s => DbFunctions.TruncateTime(s.LogDate) == searchDate);
                }
                else
                {
                    searchBox = searchBox.ToLower();
                    stockLogs = stockLogs.Where(s => s.LogNo.ToLower().Contains(searchBox)
                                || s.OldStock.ToString().Contains(searchBox)
                                || s.NumberOfChange.ToString().Contains(searchBox)
                                || s.CurrentStock.ToString().Contains(searchBox)
                                || s.Remarks.ToLower().Contains(searchBox)
                                || s.LogBy.ToLower().Contains(searchBox));
                }
            }
            if (stockLogs.Count() > 0)
            {
                stockLogs = SelectOrder(sortOrder, stockLogs);
            }
            int pageSize = GlobalValue.LargePageSize;
            int pageNumber = (page ?? 1);

            List<StockLogViewModel> viewModels = new List<StockLogViewModel>();
            // ไม่ where ด้วย !p.IsDeleted และ .Single() เพราะเป็นหน้าที่ link มาจาก Product จึงมั่นใจว่ามี productName แน่นอน
            string productName = _context.Products.Where(p => p.Id == productId).Select(p => p.Name).Single();
            if (stockLogs.Count() <= 0)
            {
                viewModels.Add(new StockLogViewModel
                {
                    ProductId = productId,
                    ProductName = productName,
                    StockLog = null
                });
            }
            else
            {
                foreach (StockLog item in stockLogs)
                {
                    viewModels.Add(new StockLogViewModel
                    {
                        ProductId = productId,
                        ProductName = productName,
                        StockLog = item
                    });
                }
            }
            return View(viewModels.ToPagedList(pageNumber, pageSize));
        }

        private static IQueryable<StockLog> SelectOrder(string sortOrder, IQueryable<StockLog> stockLogs)
        {
            switch (sortOrder)
            {
                case "log_no":
                    stockLogs = stockLogs.OrderBy(s => s.LogNo);
                    break;
                case "log_no_desc":
                    stockLogs = stockLogs.OrderByDescending(s => s.LogNo);
                    break;
                case "old_stock":
                    stockLogs = stockLogs.OrderBy(s => s.OldStock);
                    break;
                case "old_stock_desc":
                    stockLogs = stockLogs.OrderByDescending(c => c.OldStock);
                    break;
                case "no_of_change":
                    stockLogs = stockLogs.OrderBy(c => c.NumberOfChange);
                    break;
                case "no_of_change_desc":
                    stockLogs = stockLogs.OrderByDescending(c => c.NumberOfChange);
                    break;
                case "cur_stock":
                    stockLogs = stockLogs.OrderBy(c => c.CurrentStock);
                    break;
                case "cur_stock_desc":
                    stockLogs = stockLogs.OrderByDescending(c => c.CurrentStock);
                    break;
                case "log_by":
                    stockLogs = stockLogs.OrderBy(s => s.LogBy);
                    break;
                case "log_by_desc":
                    stockLogs = stockLogs.OrderByDescending(s => s.LogBy);
                    break;
                case "log_date":
                    stockLogs = stockLogs.OrderBy(s => s.LogDate);
                    break;
                case "log_date_desc":
                    stockLogs = stockLogs.OrderByDescending(s => s.LogDate);
                    break;
                case "log_type":
                    stockLogs = stockLogs.OrderBy(s => s.StockLogType.Name);
                    break;
                case "log_type_desc":
                    stockLogs = stockLogs.OrderByDescending(s => s.StockLogType.Name);
                    break;
                default:
                    stockLogs = stockLogs.OrderByDescending(c => c.Id);
                    break;
            }
            return stockLogs;
        }

        // GET: StockLogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockLog stockLog = _context.StockLogs.Include(s => s.Product).Include(s => s.StockLogType).SingleOrDefault(s => s.Id == id);
            if (stockLog == null)
            {
                return HttpNotFound();
            }
            return View(stockLog);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockLog stockLog = _context.StockLogs.Include(s => s.Product).Include(s => s.StockLogType).SingleOrDefault(s => s.Id == id);
            if (stockLog == null)
            {
                return HttpNotFound();
            }
            StockLogViewModel viewModel = new StockLogViewModel
            {
                ProductId = stockLog.ProductId,
                ProductName = stockLog.Product.Name,
                StockLog = stockLog
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StockLog stockLog)
        {
            if (!ModelState.IsValid)
            {
                StockLogViewModel viewModel = new StockLogViewModel
                {
                    ProductId = stockLog.ProductId,
                    // ไม่ where ด้วย !p.IsDeleted และ .Single() เพราะเป็นหน้าที่ link มาจาก Product จึงมั่นใจว่ามี ProductName แน่นอน
                    ProductName = _context.Products.Where(p => p.Id == stockLog.ProductId).Select(p => p.Name).Single(),
                    StockLog = stockLog
                };
                return View(viewModel);
            }
            StockLog stockLogInDb = _context.StockLogs.Single(s => s.Id == stockLog.Id);
            stockLogInDb.Remarks = stockLog.Remarks;
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = stockLog.Id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}
