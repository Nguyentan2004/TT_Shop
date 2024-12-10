using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using TT_Shop.Models;

namespace TT_Shop.Controllers
{
    public class HomeController : Controller
    {
        private QLTTShopEntities db = new QLTTShopEntities();

        private void LoadCategories()
        {
            var categories = db.Categories.ToList();
            ViewBag.Categories = categories;
        }

        public ActionResult Index(int page = 1, int pageSize = 6)
        {
            LoadCategories(); 
            var products = db.Products
                .OrderByDescending(p => p.created_at) 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            int totalProducts = db.Products.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.CurrentPage = page;
            return View(products);
        }


        //public ActionResult About()
        //{
        //    LoadCategories(); // Gọi phương thức LoadCategories để lấy danh sách Categories
        //    ViewBag.Message = "Trang mô tả.";
        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    LoadCategories(); // Gọi phương thức LoadCategories để lấy danh sách Categories
        //    ViewBag.Message = "Trang liên hệ.";
        //    return View();
        //}

        public ActionResult Detail(int id, int page = 1)
        {
            int pageSize = 5;
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var relatedProducts = db.Products
                .Where(p => p.category_id == product.category_id && p.product_id != id)
                .Take(4) 
                .ToList();

            ViewBag.RelatedProducts = relatedProducts;

            var reviews = db.Product_Reviews
                            .Where(r => r.product_id == id)
                            .OrderByDescending(r => r.created_at)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            ViewBag.Reviews = reviews;
            ViewBag.TotalPages = Math.Ceiling((double)db.Product_Reviews.Count(r => r.product_id == id) / pageSize);
            ViewBag.CurrentPage = page;

            return View(product);
        }


        [HttpPost]
        public JsonResult AddReview(int productId, int rating, string comment)
        {
            try
            {
                var user = Session["User"] as User;

                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng." });
                }

                var review = new Product_Reviews
                {
                    product_id = productId,
                    rating = rating,
                    comment = comment,
                    user_id = user.user_id,
                    created_at = DateTime.Now
                };

                db.Product_Reviews.Add(review);
                db.SaveChanges();

                return Json(new { success = true, message = "Gửi đánh giá thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Xảy ra lỗi: " + ex.Message });
            }
        }


        public ActionResult Category(int id, int page = 1)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            int pageSize = 6; 
            int totalProducts = db.Products.Count(p => p.category_id == id);

            int totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var products = db.Products
                             .Where(p => p.category_id == id)
                             .OrderBy(p => p.product_id) 
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            ViewBag.CategoryName = category.name;
            ViewBag.CategoryId = id;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(products);
        }

        public ActionResult GetProductsByCategory(int id)
        {
            var products = db.Products.Where(p => p.category_id == id).ToList();
            return PartialView("GetProductsByCategory", products);
        }
    }
}
