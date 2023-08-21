using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JackMaiMa.Models;
using PagedList;
using System.Data.Entity;
using System.Globalization;
using JackMaiMa.ViewModels;
using System.Net;
using System.IO;

namespace JackMaiMa.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private ApplicationDbContext _context;

        public ProductsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Products
        public ActionResult Index(string sortOrder, string currentFilter, string searchBox, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SortByName = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.SortByCat = sortOrder == "cat" ? "cat_desc" : "cat";
            ViewBag.SortBySup = sortOrder == "sup" ? "sup_desc" : "sup";
            ViewBag.SortByCost = sortOrder == "cost" ? "cost_desc" : "cost";
            ViewBag.SortByPrice = sortOrder == "price" ? "price_desc" : "price";
            ViewBag.SortByStock = sortOrder == "stock" ? "stock_desc" : "stock"; 
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
            IQueryable<Product> products = _context.Products.Include(p => p.Category)
                                                .Include(p => p.Supplier)
                                                .Where(p => !p.IsDeleted)
                                                .Select(p => p);
            if (!string.IsNullOrWhiteSpace(searchBox))
            {
                CultureInfo culture = CultureInfo.CreateSpecificCulture("nl-BE");
                DateTimeStyles styles = DateTimeStyles.None;
                DateTime searchDate;
                if (DateTime.TryParse(searchBox, culture, styles, out searchDate))
                {
                    searchDate = searchDate.Date;
                    products = products.Where(p => DbFunctions.TruncateTime(p.CreateDate) == searchDate
                                    || DbFunctions.TruncateTime(p.ModifiedDate) == searchDate);
                }
                else
                {
                    searchBox = searchBox.ToLower();
                    products = products.Where(p => p.Name.ToLower().Contains(searchBox)
                                    || p.Category.Name.ToLower().Contains(searchBox)
                                    || p.Supplier.Name.ToLower().Contains(searchBox)
                                    || p.Description.ToLower().Contains(searchBox)
                                    || p.UnitPrice.ToString().ToLower().Contains(searchBox)
                                    || p.NumberInStock.ToString().ToLower().Contains(searchBox)
                                    || p.NumberInSell.ToString().ToLower().Contains(searchBox)
                                    || p.Category.Name.ToLower().Contains(searchBox)
                                    || p.Supplier.Name.ToLower().Contains(searchBox)
                                    || p.CreatedBy.ToLower().Contains(searchBox)
                                    || p.ModifiedBy.ToLower().Contains(searchBox));
                }

            }
            if (products.Count() > 0)
            {
                products = SelectOrder(sortOrder, products);
            }
            int pageSize = GlobalValue.MediumPageSize;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        private static IQueryable<Product> SelectOrder(string sortOrder, IQueryable<Product> products)
        {
            switch (sortOrder)
            {
                case "name":
                    products = products.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                case "cat":
                    products = products.OrderBy(p => p.Category.Name);
                    break;
                case "cat_desc":
                    products = products.OrderByDescending(p => p.Category.Name);
                    break;
                case "sup":
                    products = products.OrderBy(p => p.Supplier.Name);
                    break;
                case "sup_desc":
                    products = products.OrderByDescending(p => p.Supplier.Name);
                    break;
                case "cost":
                    products = products.OrderBy(p => p.UnitCost);
                    break;
                case "cost_desc":
                    products = products.OrderByDescending(p => p.UnitCost);
                    break;
                case "price":
                    products = products.OrderBy(p => p.UnitPrice);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.UnitPrice);
                    break;
                case "stock":
                    products = products.OrderBy(p => p.NumberInStock);
                    break;
                case "stock_desc":
                    products = products.OrderByDescending(p => p.NumberInStock);
                    break;
                case "mod_by":
                    products = products.OrderBy(p => p.ModifiedBy);
                    break;
                case "mod_by_desc":
                    products = products.OrderByDescending(p => p.ModifiedBy);
                    break;
                case "mod_date":
                    products = products.OrderBy(p => p.ModifiedDate);
                    break;
                case "mod_date_desc":
                    products = products.OrderByDescending(p => p.ModifiedDate);
                    break;
                default:
                    products = products.OrderByDescending(c => c.Id);
                    break;
            }
            return products;
        }

        public ActionResult Create()
        {
            ProductFormViewModel viewModel = new ProductFormViewModel
            {
                Product = new Product(),
                Categories = _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToList(),
                Suppliers = _context.Suppliers.Where(s => !s.IsDeleted).OrderBy(s => s.Name).ToList()
            };
            return View("ProductForm", viewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _context.Products.SingleOrDefault(p => p.Id == id && !p.IsDeleted);
            if (product == null)
            {
                return HttpNotFound();
            }
            ProductFormViewModel viewModel = new ProductFormViewModel
            {
                Product = product,
                Categories = _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToList(),
                Suppliers = _context.Suppliers.Where(s => !s.IsDeleted).OrderBy(s => s.Name).ToList()
            };
            return View("ProductForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Product product, HttpPostedFileBase ImageFile, string changeStockAction, string changeStockAmountText, string changeStockRemarks)
        {
            ProductFormViewModel viewModel;
            if (!ModelState.IsValid)
            {
                viewModel = new ProductFormViewModel
                {
                    Product = product,
                    Categories = _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToList(),
                    Suppliers = _context.Suppliers.Where(s => !s.IsDeleted).OrderBy(s => s.Name).ToList()
                };
                return View("ProductForm", viewModel);
            }
            bool isNewArrival = false;
            if (product.Id == 0)
            {
                isNewArrival = true;
                if (ImageFile != null)
                {
                    string pic = SaveImageToServer(ImageFile);
                    product.ImageUrl = "~/Images/" + pic;
                    product.IsNoImage = false;
                }
                else
                {
                    product.IsNoImage = true;
                }
                product.CreateDate = DateTime.Now;
                product.ModifiedDate = DateTime.Now;
                _context.Products.Add(product);
            }
            else
            {
                Product prodInDb = _context.Products.Single(p => p.Id == product.Id);
                if (!product.IsNoImage)
                {
                    if (ImageFile != null)
                    {
                        string pic = SaveImageToServer(ImageFile);
                        product.ImageUrl = "~/Images/" + pic;
                        ViewBag.ImageUrlChange = true;
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(prodInDb.ImageUrl))
                        {
                            product.ImageUrl = prodInDb.ImageUrl;
                        }
                        else
                        {
                            ModelState.AddModelError("Product.IsNoImage", "ระบุว่า 'มีรูปภาพ'");
                            ModelState.AddModelError("Product.ImageUrl", "ระบุว่า 'มีรูปภาพ' แต่ยังไม่ได้ Upload ไฟล์รูป");
                            viewModel = new ProductFormViewModel
                            {
                                Product = product,
                                Categories = _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToList(),
                                Suppliers = _context.Suppliers.Where(s => !s.IsDeleted).OrderBy(s => s.Name).ToList()
                            };
                            return View("ProductForm", viewModel);
                        }
                    }
                }
                if (prodInDb.Stamp.SequenceEqual(product.Stamp))
                {
                    prodInDb.Name = product.Name;
                    prodInDb.CategoryId = product.CategoryId;
                    prodInDb.SupplierId = product.SupplierId;
                    prodInDb.Description = product.Description;
                    prodInDb.UnitCost = product.UnitCost;
                    prodInDb.UnitPrice = product.UnitPrice;

                    if (changeStockAction == GlobalValue.AddStock || changeStockAction == GlobalValue.DeleteStock)
                    {
                        int changeStockAmount = Int32.Parse(changeStockAmountText);
                        if (changeStockAmount <= 0)
                        {
                            ModelState.AddModelError("changeStockAmountText", "ถ้ามีการเปลี่ยนจำนวนของใน Stock จำเป็นต้องใส่ตัวเลขที่มีค่ามากกว่า 0");
                            viewModel = new ProductFormViewModel
                            {
                                Product = product,
                                Categories = _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToList(),
                                Suppliers = _context.Suppliers.Where(s => !s.IsDeleted).OrderBy(s => s.Name).ToList()
                            };
                            return View("ProductForm", viewModel);
                        }
                        if (changeStockAction == GlobalValue.DeleteStock)
                        {
                            if (changeStockAmount > prodInDb.NumberInStock)
                            {
                                ModelState.AddModelError("Product.NumberInStock", "จำนวนของใน Stock ที่มีอยู่");
                                ModelState.AddModelError("changeStockAmountText", "การลบจำนวนของใน Stock ไม่สามารถลบมากกว่าจำนวนที่มีอยู่ได้");
                                viewModel = new ProductFormViewModel
                                {
                                    Product = product,
                                    Categories = _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToList(),
                                    Suppliers = _context.Suppliers.Where(s => !s.IsDeleted).OrderBy(s => s.Name).ToList()
                                };
                                return View("ProductForm", viewModel);
                            }
                            changeStockAmount *= -1;
                        }
                        if (String.IsNullOrWhiteSpace(changeStockRemarks))
                        {
                            ModelState.AddModelError("changeStockRemarks", "ถ้ามีการเปลี่ยนจำนวนของใน Stock จำเป็นต้องระบุสาเหตุ");
                            viewModel = new ProductFormViewModel
                            {
                                Product = product,
                                Categories = _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToList(),
                                Suppliers = _context.Suppliers.Where(s => !s.IsDeleted).OrderBy(s => s.Name).ToList()
                            };
                            return View("ProductForm", viewModel);
                        }
                        int oldStock = prodInDb.NumberInStock;
                        prodInDb.NumberInStock += changeStockAmount;
                        int runNo = _context.StockLogs.Where(s => s.ProductId == product.Id).Max(s => s.RunningNo);
                        runNo++;
                        StockLog log = new StockLog()
                        {
                            RunningNo = runNo,
                            LogNo = String.Format("PD{0}-{1}", product.Id, runNo),
                            ProductId = product.Id,
                            StockLogTypeId = StockLogType.Edit,
                            OldStock = oldStock,
                            NumberOfChange = changeStockAmount,
                            CurrentStock = prodInDb.NumberInStock,
                            Remarks = changeStockRemarks,
                            LogDate = DateTime.Now,
                            LogBy = product.ModifiedBy
                        };
                        _context.StockLogs.Add(log);
                    }
                 
                    ManageImageData(product, prodInDb);
                    prodInDb.ModifiedBy = product.ModifiedBy;
                    prodInDb.ModifiedDate = DateTime.Now;
                }
                else
                {
                    ReportDbUpdateConcurrencyException(product);
                    viewModel = new ProductFormViewModel
                    {
                        Product = product,
                        Categories = _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToList(),
                        Suppliers = _context.Suppliers.Where(s => !s.IsDeleted).OrderBy(s => s.Name).ToList()
                    };
                    return View("ProductForm", viewModel);
                }
            }
            _context.SaveChanges();
            if (isNewArrival)
            {
                int productId = _context.Products.Max(p => p.Id);
                Product newProduct = _context.Products.Single(p => p.Id == productId);
                StockLog log = new StockLog()
                {
                    RunningNo = GlobalValue.StartRunningNo,
                    LogNo = String.Format("PD{0}-{1}", productId, GlobalValue.StartRunningNo),
                    ProductId = productId,
                    StockLogTypeId = StockLogType.Edit,
                    OldStock = 0,
                    NumberOfChange = product.NumberInStock,
                    CurrentStock = product.NumberInStock,
                    Remarks = "เป็น Log ตอนเริ่มต้นสร้างข้อมูล",
                    LogDate = newProduct.CreateDate,
                    LogBy = product.CreatedBy
                };
                _context.StockLogs.Add(log);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        private void ReportDbUpdateConcurrencyException(Product product)
        {
            Product dbValue = _context.Products.SingleOrDefault(c => c.Id == product.Id);
            string updateUser = "User คนอื่น";
            bool isProdExist = false;
            if (dbValue != null)
            {
                if (!String.IsNullOrWhiteSpace(dbValue.ModifiedBy))
                {
                    updateUser = string.Format("User '{0}'", dbValue.ModifiedBy);
                }
                Product dbValueAgain = _context.Products.SingleOrDefault(c => c.Id == product.Id && !c.IsDeleted);
                if (dbValueAgain != null)
                {
                    isProdExist = true;
                }
            }
            if (!isProdExist)
            {
                ModelState.AddModelError(string.Empty, string.Format("ไม่สามารถ Save ได้เพราะ {0} ลบรายการนี้แล้ว", updateUser));
            }
            else
            {
                if (dbValue.Name != product.Name)
                {
                    ModelState.AddModelError("Product.Name", "ค่าล่าสุด : " + dbValue.Name);
                }
                if (dbValue.Description != product.Description)
                {
                    ModelState.AddModelError("Product.Description", "ค่าล่าสุด : " + dbValue.Description);
                }
                if (dbValue.UnitPrice != product.UnitPrice)
                {
                    ModelState.AddModelError("Product.UnitPrice", "ค่าล่าสุด : " + dbValue.UnitPrice);
                }
                if (dbValue.NumberInStock != product.NumberInStock)
                {
                    ModelState.AddModelError("Product.NumberInStock", "ค่าล่าสุด : " + dbValue.NumberInStock);
                }
                if (dbValue.NumberInSell != product.NumberInSell)
                {
                    ModelState.AddModelError("Product.NumberInSell", "ค่าล่าสุด : " + dbValue.NumberInSell);
                }
                if (dbValue.IsNoImage != product.IsNoImage)
                {
                    string isNoImgText = dbValue.IsNoImage ? "ไม่มีรูปภาพ" : "มีรูปภาพ";
                    ModelState.AddModelError("Product.IsNoImage", "ค่าล่าสุด : " + isNoImgText);
                }
                if (!product.IsNoImage && (dbValue.ImageUrl != product.ImageUrl))
                {
                    string imgUrlText = String.IsNullOrWhiteSpace(dbValue.ImageUrl) ? "ไม่มี Image Url" : dbValue.ImageUrl;
                    ModelState.AddModelError("Product.ImageUrl", "ค่าล่าสุด : " + imgUrlText);
                }
                if (dbValue.CategoryId != product.CategoryId)
                {
                    ModelState.AddModelError("Product.CategoryId", "ค่าล่าสุด : " + dbValue.CategoryId);
                }
                if (dbValue.SupplierId != product.SupplierId)
                {
                    ModelState.AddModelError("Product.SupplierId", "ค่าล่าสุด : " + dbValue.SupplierId);
                }
                ModelState.AddModelError(string.Empty
                    , string.Format("รายการที่คุณกำลังแก้ไข มี {0} แก้ไขพร้อมกัน ถ้ายืนยันที่จะแก้ไข โปรดตรวจสอบข้อมูลอีกครั้งก่อนกด Save ถ้ายกเลิกการแก้ไข คลิกที่ปุ่ม 'กลับสู่หน้ารายชื่อ'", updateUser));
                product.Stamp = dbValue.Stamp;
            }
        }

        private void ManageImageData(Product product, Product prodInDb)
        {
            if (product.IsNoImage)
            {
                if (!prodInDb.IsNoImage)
                {
                    prodInDb.ImageUrl = null;
                    prodInDb.IsNoImage = true;
                }
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(product.ImageUrl))
                {
                    prodInDb.IsNoImage = false;
                    prodInDb.ImageUrl = product.ImageUrl;
                }
            }
        }

        private string SaveImageToServer(HttpPostedFileBase imageFile)
        {
            string pic = Path.GetFileName(imageFile.FileName);
            string path = Path.Combine(Server.MapPath("~/Images/"), pic);
            imageFile.SaveAs(path);
            return pic;
        }

        private void UpdateProductLog(bool isAdd, Product product, string currentUser)
        {
            product.ModifiedDate = DateTime.Now;
            product.ModifiedBy = currentUser;
            if (isAdd)
            {
                product.CreateDate = DateTime.Now;
                product.CreatedBy = currentUser;
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

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _context.Products.Include(p => p.Category).Include(p => p.Supplier).SingleOrDefault(p => p.Id == id && !p.IsDeleted);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _context.Products.Include(p => p.Category).Include(p => p.Supplier).SingleOrDefault(p => p.Id == id && !p.IsDeleted);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            string currentUser = GetCurrentUser();
            Product prodInDb = _context.Products.Single(p => p.Id == id);
            prodInDb.IsDeleted = true;
            UpdateProductLog(false, prodInDb, currentUser);
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