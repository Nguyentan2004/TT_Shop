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

        private void LoadCategories()
        {
            ViewBag.Categories = db.Categories.ToList();
        }

        public ActionResult Index(int page = 1, int pageSize = 6)
        {
            LoadCategories();
            var products = db.Products.OrderBy(p => p.product_id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            int totalProducts = db.Products.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.CurrentPage = page;

            var viewModel = new HomeViewModel
            {
                Products = products,
                Categories = db.Categories.ToList()
            };

            return View(viewModel);
        }

        public ActionResult Categories()
        {
            LoadCategories();
            return PartialView("_Categories");
        }

        public ActionResult About()
        {
            LoadCategories();
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            LoadCategories();
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Detail(int id)
        {
            LoadCategories();
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
    }
}
