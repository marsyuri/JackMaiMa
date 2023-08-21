using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JackMaiMa.Models;
using PagedList;
using System.Globalization;
using System.Net;
using System.Data.Entity;
using JackMaiMa.ViewModels;

namespace JackMaiMa.Controllers
{
    [Authorize]
    public class SuppliersController : Controller
    {
        private ApplicationDbContext _context;

        public SuppliersController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Suppliers
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
            IQueryable<Supplier> suppliers = _context.Suppliers.Where(s => !s.IsDeleted).Select(s => s);
            if (!string.IsNullOrWhiteSpace(searchBox))
            {
                CultureInfo culture = CultureInfo.CreateSpecificCulture("nl-BE");
                DateTimeStyles styles = DateTimeStyles.None;
                DateTime searchDate;
                if (DateTime.TryParse(searchBox, culture, styles, out searchDate))
                {
                    searchDate = searchDate.Date;
                    suppliers = suppliers.Where(s => DbFunctions.TruncateTime(s.CreateDate) == searchDate
                                    || DbFunctions.TruncateTime(s.ModifiedDate) == searchDate);
                }
                else
                {
                    searchBox = searchBox.ToLower();
                    suppliers = suppliers.Where(s => s.Name.ToLower().Contains(searchBox)
                                    || s.ContactName.ToLower().Contains(searchBox)
                                    || s.ContactTitle.ToLower().Contains(searchBox)
                                    || s.Address.ToLower().Contains(searchBox)
                                    || s.Tambon.ToLower().Contains(searchBox)
                                    || s.Amphur.ToLower().Contains(searchBox)
                                    || s.Province.ToLower().Contains(searchBox)
                                    || s.PostalCode.ToLower().Contains(searchBox)
                                    || s.Phone.ToLower().Contains(searchBox)
                                    || s.Email.ToLower().Contains(searchBox)
                                    || s.OptionalContact.ToLower().Contains(searchBox)
                                    || s.CreatedBy.ToLower().Contains(searchBox)
                                    || s.ModifiedBy.ToLower().Contains(searchBox));
                }
            }
            if (suppliers.Count() > 0)
            {
                suppliers = SelectOrder(sortOrder, suppliers);
            }
            int pageSize = GlobalValue.MediumPageSize;
            int pageNumber = (page ?? 1);
            return View(suppliers.ToPagedList(pageNumber, pageSize));
        }

        private IQueryable<Supplier> SelectOrder(string sortOrder, IQueryable<Supplier> suppliers)
        {
            switch (sortOrder)
            {
                case "name":
                    suppliers = suppliers.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    suppliers = suppliers.OrderByDescending(s => s.Name);
                    break;
                case "ctc_name":
                    suppliers = suppliers.OrderBy(s => s.ContactName);
                    break;
                case "ctc_name_desc":
                    suppliers = suppliers.OrderByDescending(s => s.ContactName);
                    break;
                case "ctc_title":
                    suppliers = suppliers.OrderBy(s => s.ContactTitle);
                    break;
                case "ctc_title_desc":
                    suppliers = suppliers.OrderByDescending(s => s.ContactTitle);
                    break;
                case "phone":
                    suppliers = suppliers.OrderBy(s => s.Phone);
                    break;
                case "phone_desc":
                    suppliers = suppliers.OrderByDescending(s => s.Phone);
                    break;
                case "email":
                    suppliers = suppliers.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    suppliers = suppliers.OrderByDescending(s => s.Email);
                    break;
                case "mod_by":
                    suppliers = suppliers.OrderBy(s => s.ModifiedBy);
                    break;
                case "mod_by_desc":
                    suppliers = suppliers.OrderByDescending(s => s.ModifiedBy);
                    break;
                case "mod_date":
                    suppliers = suppliers.OrderBy(s => s.ModifiedDate);
                    break;
                case "mod_date_desc":
                    suppliers = suppliers.OrderByDescending(s => s.ModifiedDate);
                    break;
                default:
                    suppliers = suppliers.OrderByDescending(c => c.Id);
                    break;
            }
            return suppliers;
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = _context.Suppliers.SingleOrDefault(s => s.Id == id && !s.IsDeleted);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            SupplierDetailViewModel viewModel = new SupplierDetailViewModel
            {
                Supplier = supplier,
                Products = _context.Products.Include(p => p.Category)
                                .Where(p => p.SupplierId == id && !p.IsDeleted)
                                .OrderBy(p => p.Name)
                                .ToList()

            };
            return View(viewModel);
        }

        public ActionResult Create()
        {
            return View("SupplierForm");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = _context.Suppliers.SingleOrDefault(s => s.Id == id && !s.IsDeleted);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View("SupplierForm", supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return View("SupplierForm", supplier);
            }
            string currentUser = GetCurrentUser();
            if (supplier.Id == 0)
            {
                supplier.CreateDate = DateTime.Now;
                supplier.ModifiedDate = DateTime.Now;
                _context.Suppliers.Add(supplier);
            }
            else
            {
                Supplier supInDb = _context.Suppliers.Single(c => c.Id == supplier.Id);
                if (supInDb.Stamp.SequenceEqual(supplier.Stamp))
                {
                    supInDb.Name = supplier.Name;
                    supInDb.ContactName = supplier.ContactName;
                    supInDb.ContactTitle = supplier.ContactTitle;
                    supInDb.Address = supplier.Address;
                    supInDb.Tambon = supplier.Tambon;
                    supInDb.Amphur = supplier.Amphur;
                    supInDb.Province = supplier.Province;
                    supInDb.PostalCode = supplier.PostalCode;
                    supInDb.Phone = supplier.Phone;
                    supInDb.Email = supplier.Email;
                    supInDb.OptionalContact = supplier.OptionalContact;
                    supInDb.ModifiedBy = supplier.ModifiedBy;
                    supInDb.ModifiedDate = DateTime.Now;
                }
                else
                {
                    ReportDbUpdateConcurrencyException(supplier);
                    return View("SupplierForm", supplier);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private void ReportDbUpdateConcurrencyException(Supplier supplier)
        {
            Supplier dbValue = _context.Suppliers.SingleOrDefault(s => s.Id == supplier.Id);
            string updateUser = "User คนอื่น";
            bool isSupExist = false;
            if (dbValue != null)
            {
                if (!String.IsNullOrWhiteSpace(dbValue.ModifiedBy))
                {
                    updateUser = string.Format("User '{0}'", dbValue.ModifiedBy);
                }
                Supplier dbValueAgain = _context.Suppliers.SingleOrDefault(s => s.Id == supplier.Id && !s.IsDeleted);
                if (dbValueAgain != null)
                {
                    isSupExist = true;
                }
            }
            if (!isSupExist)
            {
                ModelState.AddModelError(string.Empty, string.Format("ไม่สามารถ Save ได้เพราะ {0} ลบรายการนี้แล้ว", updateUser));
            }
            else
            {
                if (dbValue.Name != supplier.Name)
                {
                    ModelState.AddModelError("Name", "ค่าล่าสุด : " + dbValue.Name);
                }
                if (dbValue.ContactName != supplier.ContactName)
                {
                    ModelState.AddModelError("ContactName", "ค่าล่าสุด : " + dbValue.ContactName);
                }
                if (dbValue.ContactTitle != supplier.ContactTitle)
                {
                    ModelState.AddModelError("ContactTitle", "ค่าล่าสุด : " + dbValue.ContactTitle);
                }
                if (dbValue.Address != supplier.Address)
                {
                    ModelState.AddModelError("Address", "ค่าล่าสุด : " + dbValue.Address);
                }
                if (dbValue.Tambon != supplier.Tambon)
                {
                    ModelState.AddModelError("Tambon", "ค่าล่าสุด : " + dbValue.Tambon);
                }
                if (dbValue.Amphur != supplier.Amphur)
                {
                    ModelState.AddModelError("Amphur", "ค่าล่าสุด : " + dbValue.Amphur);
                }
                if (dbValue.Province != supplier.Province)
                {
                    ModelState.AddModelError("Province", "ค่าล่าสุด : " + dbValue.Province);
                }
                if (dbValue.PostalCode != supplier.PostalCode)
                {
                    ModelState.AddModelError("PostalCode", "ค่าล่าสุด : " + dbValue.PostalCode);
                }
                if (dbValue.Phone != supplier.Phone)
                {
                    ModelState.AddModelError("Phone", "ค่าล่าสุด : " + dbValue.Phone);
                }
                if (dbValue.Email != supplier.Email)
                {
                    ModelState.AddModelError("Email", "ค่าล่าสุด : " + dbValue.Email);
                }
                if (dbValue.OptionalContact != supplier.OptionalContact)
                {
                    ModelState.AddModelError("OptionalContact", "ค่าล่าสุด : " + dbValue.OptionalContact);
                }
                ModelState.AddModelError(string.Empty
                    , string.Format("รายการที่คุณกำลังแก้ไข มี {0} แก้ไขพร้อมกัน ถ้ายืนยันที่จะแก้ไข โปรดตรวจสอบข้อมูลอีกครั้งก่อนกด Save ถ้ายกเลิกการแก้ไข คลิกที่ปุ่ม 'กลับสู่หน้ารายชื่อ'", updateUser));
                supplier.Stamp = dbValue.Stamp;
            }
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

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = _context.Suppliers.SingleOrDefault(s => s.Id == id && !s.IsDeleted);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            IQueryable<Product> products = _context.Products.Where(p => p.SupplierId == id);
            if (products.Count() > 0)
            {
                ViewBag.IsRelateProduct = true;
            }
            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Supplier supInDb = _context.Suppliers.Single(s => s.Id == id);
            supInDb.IsDeleted = true;
            supInDb.ModifiedBy = GetCurrentUser();
            supInDb.ModifiedDate = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("Index");
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