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

        public ActionResult Index()
        {
            var allProducts = db.Products.ToList();
            return View(allProducts);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Detail(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.product_id == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var reviews = db.Product_Reviews.Where(r => r.product_id == id).ToList();

            return View(product);
        }
    }

}