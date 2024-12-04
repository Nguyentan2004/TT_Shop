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
            var products = db.Products
                .OrderByDescending(p => p.created_at) // Order by created_at in descending order
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            int totalProducts = db.Products.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.CurrentPage = page;
            return View(products);
        }


        public ActionResult About()
        {
            LoadCategories(); // Gọi phương thức LoadCategories để lấy danh sách Categories
            ViewBag.Message = "Trang mô tả.";
            return View();
        }

        public ActionResult Contact()
        {
            LoadCategories(); // Gọi phương thức LoadCategories để lấy danh sách Categories
            ViewBag.Message = "Trang liên hệ.";
            return View();
        }

        public ActionResult Detail(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Fetch related products based on category or other criteria
            var relatedProducts = db.Products
                .Where(p => p.category_id == product.category_id && p.product_id != id)
                .Take(4) // Limit the number of related products
                .ToList();

            ViewBag.RelatedProducts = relatedProducts;

            // Fetch reviews for the product including user information
            var reviews = db.Product_Reviews
                            .Include("User")
                            .Where(r => r.product_id == id)
                            .ToList();
            ViewBag.Reviews = reviews;

            return View(product);
        }
        
        [HttpPost]
        public ActionResult AddReview(int productId, int rating, string comment, string userName)
        {
            try
            {
                // Get the logged-in user from the session
                var user = Session["User"] as User;

                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("User not found in session.");
                    ViewBag.ErrorMessage = "Không tìm thấy người dùng.";
                    return View("Error");
                }

                System.Diagnostics.Debug.WriteLine("User found: " + user.user_id);

                // Create a new review
                var review = new Product_Reviews
                {
                    product_id = productId,
                    rating = rating,
                    comment = comment,
                    user_id = user.user_id, // Associate the review with the logged-in user
                    created_at = DateTime.Now
                };

                // Save the review to the database
                db.Product_Reviews.Add(review);
                db.SaveChanges();

                // Log success
                System.Diagnostics.Debug.WriteLine("Review saved successfully.");

                // Prepare the data to be displayed
                var reviewData = new
                {
                    userName = user.fullname,
                    rating = review.rating,
                    comment = review.comment,
                    createdAt = review.created_at
                };

                // Return a partial view with the review data
                return PartialView("ReviewPartial", reviewData);
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine("Error saving review: " + ex.Message);
                ViewBag.ErrorMessage = "Xảy ra lỗi: " + ex.Message;
                return View("Error");
            }
        }


        public ActionResult Category(int id, int page = 1)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            // Lấy tổng số sản phẩm trong danh mục
            int pageSize = 6;  // Số sản phẩm hiển thị mỗi trang
            int totalProducts = db.Products.Count(p => p.category_id == id);

            // Tính tổng số trang
            int totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            // Lấy danh sách sản phẩm theo trang
            var products = db.Products
                             .Where(p => p.category_id == id)
                             .OrderBy(p => p.product_id)  // Sắp xếp theo ID sản phẩm hoặc theo một thuộc tính khác
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            // Truyền dữ liệu sang view
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
