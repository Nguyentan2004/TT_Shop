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
    }
}
