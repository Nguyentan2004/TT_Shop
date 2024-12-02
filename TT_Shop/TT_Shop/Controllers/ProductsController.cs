using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TT_Shop.Models;

namespace TT_Shop.Controllers
{
    public class ProductsController : Controller
    {
        private QLTTShopEntities db = new QLTTShopEntities();

        public ActionResult Search(string query)
        {
            var products = db.Products
                             .Where(p => p.name.Contains(query) || p.description.Contains(query))
                             .ToList();

            return View(products);
        }

        public ActionResult Index()
        {
            var categories = db.Categories.ToList();
            ViewBag.Categories = categories;

            IEnumerable<Product> products = GetProducts();
            return View(products);
        }

        private IEnumerable<Product> GetProducts()
        {
            return db.Products.ToList();
        }

        [HttpPost]
        public ActionResult AddReview(int productId, int rating, string comment)
        {
            if (Session["user_id"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var review = new Product_Reviews
            {
                product_id = productId,
                user_id = (int)Session["user_id"],
                rating = rating,
                comment = comment,
                created_at = DateTime.Now
            };

            db.Product_Reviews.Add(review);
            db.SaveChanges();

            return RedirectToAction("Detail", "Home", new { id = productId });
        }

    }
}
