using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JackMaiMa.Models;
using PagedList;
using System.IO;
using System.Globalization;
using System.Data.Entity;
using JackMaiMa.ViewModels;

namespace JackMaiMa.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private ApplicationDbContext _context;

        public CategoriesController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index(string sortOrder, string currentFilter, string searchBox, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SortByName = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.SortByPhoto = sortOrder == "photo" ? "photo_desc" : "photo";
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
            IQueryable<Category> categories = _context.Categories.Where(c => !c.IsDeleted).Select(c => c);
            if (!string.IsNullOrWhiteSpace(searchBox))
            {
                CultureInfo culture = CultureInfo.CreateSpecificCulture("nl-BE");
                DateTimeStyles styles = DateTimeStyles.None;
                DateTime searchDate;
                if (DateTime.TryParse(searchBox, culture, styles, out searchDate))
                {
                    searchDate = searchDate.Date;
                    categories = categories.Where(c => DbFunctions.TruncateTime(c.CreateDate) == searchDate
                                    || DbFunctions.TruncateTime(c.ModifiedDate) == searchDate);
                }
                else
                {
                    searchBox = searchBox.ToLower();
                    categories = categories.Where(c => c.Name.ToLower().Contains(searchBox)
                                    || c.Description.ToLower().Contains(searchBox)
                                    || c.CreatedBy.ToLower().Contains(searchBox)
                                    || c.ModifiedBy.ToLower().Contains(searchBox));
                }
            }
            if (categories.Count() > 0)
            {
                categories = SelectOrder(sortOrder, categories);
            }
            int pageSize = GlobalValue.SmallPageSize;
            int pageNumber = (page ?? 1);
            return View(categories.ToPagedList(pageNumber, pageSize));
        }

        private static IQueryable<Category> SelectOrder(string sortOrder, IQueryable<Category> categories)
        {
            switch (sortOrder)
            {
                case "name":
                    categories = categories.OrderBy(c => c.Name);
                    break;
                case "name_desc":
                    categories = categories.OrderByDescending(c => c.Name);
                    break;
                case "photo":
                    categories = categories.OrderBy(c => c.ImageUrl);
                    break;
                case "photo_desc":
                    categories = categories.OrderByDescending(c => c.ImageUrl);
                    break;
                case "mod_by":
                    categories = categories.OrderBy(c => c.ModifiedBy);
                    break;
                case "mod_by_desc":
                    categories = categories.OrderByDescending(c => c.ModifiedBy);
                    break;
                case "mod_date":
                    categories = categories.OrderBy(c => c.ModifiedDate);
                    break;
                case "mod_date_desc":
                    categories = categories.OrderByDescending(c => c.ModifiedDate);
                    break;
                default:
                    categories = categories.OrderByDescending(c => c.Id);
                    break;
            }
            return categories;
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = _context.Categories.SingleOrDefault(c => c.Id == id && !c.IsDeleted);
            if (category == null)
            {
                return HttpNotFound();
            }
            CategoryDetailViewModel viewModel = new CategoryDetailViewModel()
            {
                Category = category,
                Products = _context.Products.Include(p => p.Supplier)
                                .Where(p => p.CategoryId == id && !p.IsDeleted)
                                .OrderBy(p => p.Name)
                                .ToList()
            };
            return View(viewModel);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View("CategoryForm");
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = _context.Categories.SingleOrDefault(c => c.Id == id && !c.IsDeleted);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View("CategoryForm", category);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Category category, HttpPostedFileBase ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View("CategoryForm", category);
            }
            if (category.Id == 0)
            {
                if (ImageFile != null)
                {
                    string pic = SaveImageToServer(ImageFile);
                    category.ImageUrl = "~/Images/" + pic;
                    category.IsNoImage = false;
                }
                else
                {
                    category.IsNoImage = true;
                }
                category.CreateDate = DateTime.Now;
                category.ModifiedDate = DateTime.Now;
                _context.Categories.Add(category);
            }
            else
            {
                Category catInDb = _context.Categories.Single(c => c.Id == category.Id);
                if (!category.IsNoImage)
                {
                    if (ImageFile != null)
                    {
                        string pic = SaveImageToServer(ImageFile);
                        category.ImageUrl = "~/Images/" + pic;
                        ViewBag.ImageUrlChange = true;
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(catInDb.ImageUrl))
                        {
                            category.ImageUrl = catInDb.ImageUrl;
                        }
                        else
                        {
                            ModelState.AddModelError("IsNoImage", "ระบุว่า 'มีรูปภาพ'");
                            ModelState.AddModelError("ImageUrl", "ระบุว่า 'มีรูปภาพ' แต่ยังไม่ได้ Upload ไฟล์รูป");
                            return View("CategoryForm", category);
                        }
                    }
                }
                if (catInDb.Stamp.SequenceEqual(category.Stamp))
                {
                    catInDb.Name = category.Name;
                    catInDb.Description = category.Description;
                    ManageImageData(category, catInDb);
                    catInDb.ModifiedBy = category.ModifiedBy;
                    catInDb.ModifiedDate = DateTime.Now;
                }
                else
                {
                    ReportDbUpdateConcurrencyException(category);
                    return View("CategoryForm", category);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private void ManageImageData(Category category, Category catInDb)
        {
            if (category.IsNoImage)
            {
                if (!catInDb.IsNoImage)
                {
                    catInDb.IsNoImage = true;
                    catInDb.ImageUrl = null;
                }
            }
            else
            {
                catInDb.IsNoImage = false;
                catInDb.ImageUrl = category.ImageUrl;
            }
        }

        private void ReportDbUpdateConcurrencyException(Category category)
        {
            Category dbValue = _context.Categories.SingleOrDefault(c => c.Id == category.Id);
            string updateUser = "User คนอื่น";
            bool isCatExist = false;
            if (dbValue != null)
            {
                if (!String.IsNullOrWhiteSpace(dbValue.ModifiedBy))
                {
                    updateUser = string.Format("User '{0}'", dbValue.ModifiedBy);
                }
                Category dbValueAgain = _context.Categories.SingleOrDefault(c => c.Id == category.Id && !c.IsDeleted);
                if (dbValueAgain != null)
                {
                    isCatExist = true;
                }
            }
            if (!isCatExist)
            {
                ModelState.AddModelError(string.Empty, string.Format("ไม่สามารถ Save ได้เพราะ {0} ลบรายการนี้แล้ว", updateUser));
            }
            else
            {
                if (dbValue.Name != category.Name)
                {
                    ModelState.AddModelError("Name", "ค่าล่าสุด : " + dbValue.Name);
                }
                if (dbValue.Description != category.Description)
                {
                    ModelState.AddModelError("Description", "ค่าล่าสุด : " + dbValue.Description);
                }
                if (dbValue.IsNoImage != category.IsNoImage)
                {
                    string isNoImgText = dbValue.IsNoImage ? "ไม่มีรูปภาพ" : "มีรูปภาพ";
                    ModelState.AddModelError("IsNoImage", "ค่าล่าสุด : " + isNoImgText);
                }
                if (!category.IsNoImage && (dbValue.ImageUrl != category.ImageUrl))
                {
                    string imgUrlText = String.IsNullOrWhiteSpace(dbValue.ImageUrl) ? "ไม่มี Image Url" : dbValue.ImageUrl;
                    ModelState.AddModelError("ImageUrl", "ค่าล่าสุด : " + imgUrlText);
                    // ViewBag.ImageUrlChange = true;
                }
                ModelState.AddModelError(string.Empty
                    , string.Format("รายการที่คุณกำลังแก้ไข มี {0} แก้ไขพร้อมกัน ถ้ายืนยันที่จะแก้ไข โปรดตรวจสอบข้อมูลอีกครั้งก่อนกด Save ถ้ายกเลิกการแก้ไข คลิกที่ปุ่ม 'กลับสู่หน้ารายชื่อ'", updateUser));
                category.Stamp = dbValue.Stamp;
            }
        }

        private string SaveImageToServer(HttpPostedFileBase imageFile)
        {
            string pic = Path.GetFileName(imageFile.FileName);
            string path = Path.Combine(Server.MapPath("~/Images/"), pic);
            imageFile.SaveAs(path);
            return pic;
        }


        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = _context.Categories.SingleOrDefault(c => c.Id == id && !c.IsDeleted);
            if (category == null)
            {
                return HttpNotFound();
            }
            IQueryable<Product> products = _context.Products.Where(p => p.CategoryId == id);
            if (products.Count() > 0)
            {
                ViewBag.IsRelateProduct = true;
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Category catInDb = _context.Categories.Single(c => c.Id == id);
            catInDb.IsDeleted = true;
            catInDb.ModifiedBy = GetCurrentUser();
            catInDb.ModifiedDate = DateTime.Now;
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
