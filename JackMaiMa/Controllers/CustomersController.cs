using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JackMaiMa.Models;
using System.Globalization;
using PagedList;
using JackMaiMa.ViewModels;

namespace JackMaiMa.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private ApplicationDbContext _context;

        public CustomersController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Customers
        public ActionResult Index(string sortOrder, string currentFilter, string searchBox, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SortByName = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.SortByCtcName = sortOrder == "ctc_name" ? "ctc_name_desc" : "ctc_name";
            ViewBag.SortByCtcTitle = sortOrder == "ctc_title" ? "ctc_title_desc" : "ctc_title";
            ViewBag.SortByPhone = sortOrder == "phone" ? "phone_desc" : "phone";
            ViewBag.SortByEmail = sortOrder == "email" ? "email_desc" : "email";
            ViewBag.SortByModBy = sortOrder == "mod_by" ? "mod_by_desc" : "mod_by";
            ViewBag.SortByModDate = sortOrder == "mod_date" ? "mod_date_desc" : "mod_date";
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
            IQueryable<Customer> customers = _context.Customers.Where(c => !c.IsDeleted).Select(c => c);
            if (!string.IsNullOrWhiteSpace(searchBox))
            {
                CultureInfo culture = CultureInfo.CreateSpecificCulture("nl-BE");
                DateTimeStyles styles = DateTimeStyles.None;
                DateTime searchDate;
                if (DateTime.TryParse(searchBox, culture, styles, out searchDate))
                {
                    searchDate = searchDate.Date;
                    customers = customers.Where(s => DbFunctions.TruncateTime(s.CreateDate) == searchDate
                                    || DbFunctions.TruncateTime(s.ModifiedDate) == searchDate);
                }
                else
                {
                    searchBox = searchBox.ToLower();
                    customers = customers.Where(c => c.Name.ToLower().Contains(searchBox)
                                    || c.ContactName.ToLower().Contains(searchBox)
                                    || c.ContactTitle.ToLower().Contains(searchBox)
                                    || c.Address.ToLower().Contains(searchBox)
                                    || c.Tambon.ToLower().Contains(searchBox)
                                    || c.Amphur.ToLower().Contains(searchBox)
                                    || c.Province.ToLower().Contains(searchBox)
                                    || c.PostalCode.ToLower().Contains(searchBox)
                                    || c.Phone.ToLower().Contains(searchBox)
                                    || c.Email.ToLower().Contains(searchBox)
                                    || c.OptionalContact.ToLower().Contains(searchBox)
                                    || c.CreatedBy.ToLower().Contains(searchBox)
                                    || c.ModifiedBy.ToLower().Contains(searchBox));
                }
            }
            if (customers.Count() > 0)
            {
                customers = SelectOrder(sortOrder, customers);
            }
            int pageSize = GlobalValue.MediumPageSize;
            int pageNumber = (page ?? 1);
            return View(customers.ToPagedList(pageNumber, pageSize));
        }

        private IQueryable<Customer> SelectOrder(string sortOrder, IQueryable<Customer> customers)
        {
            switch (sortOrder)
            {
                case "name":
                    customers = customers.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    customers = customers.OrderByDescending(s => s.Name);
                    break;
                case "ctc_name":
                    customers = customers.OrderBy(s => s.ContactName);
                    break;
                case "ctc_name_desc":
                    customers = customers.OrderByDescending(s => s.ContactName);
                    break;
                case "ctc_title":
                    customers = customers.OrderBy(s => s.ContactTitle);
                    break;
                case "ctc_title_desc":
                    customers = customers.OrderByDescending(s => s.ContactTitle);
                    break;
                case "phone":
                    customers = customers.OrderBy(s => s.Phone);
                    break;
                case "phone_desc":
                    customers = customers.OrderByDescending(s => s.Phone);
                    break;
                case "email":
                    customers = customers.OrderBy(c => c.Email);
                    break;
                case "email_desc":
                    customers = customers.OrderByDescending(c => c.Email);
                    break;
                case "mod_by":
                    customers = customers.OrderBy(s => s.ModifiedBy);
                    break;
                case "mod_by_desc":
                    customers = customers.OrderByDescending(s => s.ModifiedBy);
                    break;
                case "mod_date":
                    customers = customers.OrderBy(s => s.ModifiedDate);
                    break;
                case "mod_date_desc":
                    customers = customers.OrderByDescending(s => s.ModifiedDate);
                    break;
                default:
                    customers = customers.OrderByDescending(c => c.Id);
                    break;
            }
            return customers;
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = _context.Customers.SingleOrDefault(c => c.Id == id && !c.IsDeleted);
            if (customer == null)
            {
                return HttpNotFound();
            }
            CustomerDetailViewModel viewModel = new CustomerDetailViewModel
            {
                Customer = customer,
                Orders = _context.Orders.Where(o => o.CustomerId == id).OrderByDescending(o => o.Id).ToList()
            };
            return View(viewModel);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View("CustomerForm");
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = _context.Customers.SingleOrDefault(c => c.Id == id && !c.IsDeleted);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View("CustomerForm", customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View("CustomerForm", customer);
            }
            if (customer.Id == 0)
            {
                customer.CreateDate = DateTime.Now;
                customer.ModifiedDate = DateTime.Now;
                _context.Customers.Add(customer);
            }
            else
            {
                Customer custInDb = _context.Customers.Single(c => c.Id == customer.Id);
                if (custInDb.Stamp.SequenceEqual(customer.Stamp))
                {
                    custInDb.Name = customer.Name;
                    custInDb.ContactName = customer.ContactName;
                    custInDb.ContactTitle = customer.ContactTitle;
                    custInDb.Address = customer.Address;
                    custInDb.Tambon = customer.Tambon;
                    custInDb.Amphur = customer.Amphur;
                    custInDb.Province = customer.Province;
                    custInDb.PostalCode = customer.PostalCode;
                    custInDb.Phone = customer.Phone;
                    custInDb.Email = customer.Email;
                    custInDb.OptionalContact = customer.OptionalContact;
                    custInDb.ModifiedBy = customer.ModifiedBy;
                    custInDb.ModifiedDate = DateTime.Now;
                }
                else
                {
                    ReportDbUpdateConcurrencyException(customer);
                    return View("CustomerForm", customer);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private void ReportDbUpdateConcurrencyException(Customer customer)
        {
            string updateUser = "User คนอื่น";
            bool isCustExist = false;
            Supplier dbValue = _context.Suppliers.SingleOrDefault(c => c.Id == customer.Id);
            if (dbValue != null)
            {
                if (!String.IsNullOrWhiteSpace(dbValue.ModifiedBy))
                {
                    updateUser = string.Format("User '{0}'", dbValue.ModifiedBy);
                }
                Supplier dbValueAgain = _context.Suppliers.SingleOrDefault(s => s.Id == customer.Id && !s.IsDeleted);
                if (dbValueAgain != null)
                {
                    isCustExist = true;
                }
            }
            if (!isCustExist)
            {
                ModelState.AddModelError(string.Empty, string.Format("ไม่สามารถ Save ได้เพราะ {0} ลบรายการนี้แล้ว", updateUser));
            }
            else
            {
                if (dbValue.Name != customer.Name)
                {
                    ModelState.AddModelError("Name", "ค่าล่าสุด : " + dbValue.Name);
                }
                if (dbValue.ContactName != customer.ContactName)
                {
                    ModelState.AddModelError("ContactName", "ค่าล่าสุด : " + dbValue.ContactName);
                }
                if (dbValue.ContactTitle != customer.ContactTitle)
                {
                    ModelState.AddModelError("ContactTitle", "ค่าล่าสุด : " + dbValue.ContactTitle);
                }
                if (dbValue.Address != customer.Address)
                {
                    ModelState.AddModelError("Address", "ค่าล่าสุด : " + dbValue.Address);
                }
                if (dbValue.Tambon != customer.Tambon)
                {
                    ModelState.AddModelError("Tambon", "ค่าล่าสุด : " + dbValue.Tambon);
                }
                if (dbValue.Amphur != customer.Amphur)
                {
                    ModelState.AddModelError("Amphur", "ค่าล่าสุด : " + dbValue.Amphur);
                }
                if (dbValue.Province != customer.Province)
                {
                    ModelState.AddModelError("Province", "ค่าล่าสุด : " + dbValue.Province);
                }
                if (dbValue.PostalCode != customer.PostalCode)
                {
                    ModelState.AddModelError("PostalCode", "ค่าล่าสุด : " + dbValue.PostalCode);
                }
                if (dbValue.Phone != customer.Phone)
                {
                    ModelState.AddModelError("Phone", "ค่าล่าสุด : " + dbValue.Phone);
                }
                if (dbValue.Email != customer.Email)
                {
                    ModelState.AddModelError("Email", "ค่าล่าสุด : " + dbValue.Email);
                }
                if (dbValue.OptionalContact != customer.OptionalContact)
                {
                    ModelState.AddModelError("OptionalContact", "ค่าล่าสุด : " + dbValue.OptionalContact);
                }
                ModelState.AddModelError(string.Empty
                    , string.Format("รายการที่คุณกำลังแก้ไข มี {0} แก้ไขพร้อมกัน ถ้ายืนยันที่จะแก้ไข โปรดตรวจสอบข้อมูลอีกครั้งก่อนกด Save ถ้ายกเลิกการแก้ไข คลิกที่ปุ่ม 'กลับสู่หน้ารายชื่อ'", updateUser));
                customer.Stamp = dbValue.Stamp;
            }
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = _context.Customers.SingleOrDefault(c => c.Id == id && !c.IsDeleted);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Customer cusInDb = _context.Customers.Single(c => c.Id == id);
            cusInDb.IsDeleted = true;
            cusInDb.ModifiedBy = GetCurrentUser();
            cusInDb.ModifiedDate = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("Index");
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
