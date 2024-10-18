using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TT_Shop.Models; // Adjust the namespace according to your project

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
    }
}