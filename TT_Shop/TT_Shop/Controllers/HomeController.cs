using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TT_Shop.Models;

namespace TT_Shop.Controllers
{
    public class HomeController : Controller
    {
        private QLTTShopEntities db = new QLTTShopEntities();

        // Phương thức để lấy danh sách Categories và lưu trữ trong ViewBag
        private List<Category> LoadCategories()
        {
            return db.Categories.ToList();
        }

        public ActionResult Index(int page = 1, int pageSize = 6)
        {
            var categories = db.Categories.ToList();
            var products = db.Products.OrderBy(p => p.product_id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            int totalProducts = db.Products.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.CurrentPage = page;

            var viewModel = new HomeViewModel
            {
                Products = products,
                Categories = categories
            };

            return View(viewModel);
        }


        public ActionResult About()
        {
            LoadCategories(); // Gọi phương thức LoadCategories để lấy danh sách Categories
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            LoadCategories(); // Gọi phương thức LoadCategories để lấy danh sách Categories
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Detail(int id)
        {
            LoadCategories(); // Gọi phương thức LoadCategories để lấy danh sách Categories
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult Category(int id, int page = 1, int pageSize = 6)
        {
            LoadCategories(); // Gọi phương thức LoadCategories để lấy danh sách Categories
            var category = db.Categories.Include("Products").FirstOrDefault(c => c.category_id == id);
            if (category == null)
            {
                return HttpNotFound();
            }

            var products = category.Products.OrderBy(p => p.product_id)
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToList();
            int totalProducts = category.Products.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.CategoryName = category.name;

            var viewModel = new HomeViewModel
            {
                Products = products,
                Categories = db.Categories.ToList()
            };

            return View(viewModel);
        }

        public ActionResult GetProductsByCategory(int id)
        {
            var products = db.Products.Where(p => p.category_id == id).ToList();
            return PartialView("GetProductsByCategory", products);
        }
    }
}
