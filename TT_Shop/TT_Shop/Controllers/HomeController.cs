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
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Lấy đánh giá
            var reviews = db.Product_Reviews.Where(r => r.product_id == id).ToList();
            ViewBag.Reviews = reviews;

            // Lấy sản phẩm liên quan
            var relatedProducts = db.Products
                .Where(p => p.category_id == product.category_id && p.category_id != id)
                .Take(4) // Lấy 4 sản phẩm liên quan
                .ToList();
            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }

        [HttpPost]
        public ActionResult AddReview(int productId, int rating, string comment)
        {
            var review = new Product_Reviews
            {
                product_id = productId,
                rating = rating,
                comment = comment,
                created_at = DateTime.Now
            };

            db.Product_Reviews.Add(review);
            db.SaveChanges();

            return RedirectToAction("Detail", new { id = productId });
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
