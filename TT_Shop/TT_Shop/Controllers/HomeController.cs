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
        private void LoadCategories()
        {
            var categories = db.Categories.ToList();
            ViewBag.Categories = categories;
        }


        public ActionResult Index(int page = 1, int pageSize = 6)
        {
            LoadCategories(); // Call LoadCategories to populate ViewBag.Categories
            var products = db.Products.OrderBy(p => p.product_id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            int totalProducts = db.Products.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.CurrentPage = page;
            return View(products);
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

        public ActionResult Category(int id)
        {
            LoadCategories(); // Gọi phương thức LoadCategories để lấy danh sách Categories
            var products = db.Products.Where(p => p.category_id == id).ToList();
            return View(products);
        }
        public ActionResult GetProductsByCategory(int id)
        {
            var products = db.Products.Where(p => p.category_id == id).ToList();
            return PartialView("GetProductsByCategory", products);
        }

    }
}
