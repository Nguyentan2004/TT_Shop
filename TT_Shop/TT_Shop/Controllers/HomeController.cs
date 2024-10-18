﻿using System;
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
            try
            {
                var categories = db.Categories.ToList(); // Lấy tất cả các Category từ cơ sở dữ liệu
                ViewBag.Categories = categories; // Truyền danh sách Category đến view
            }
            catch (Exception ex)
            {
                // Log lỗi (bạn có thể sử dụng bất kỳ công cụ log nào bạn muốn, ví dụ: log4net, NLog, v.v.)
                System.Diagnostics.Debug.WriteLine("Error loading categories: " + ex.Message);
                // Hoặc bạn có thể lưu lỗi vào cơ sở dữ liệu hoặc file log
            }
        }


        public ActionResult Index()
        {
            LoadCategories(); // Gọi phương thức LoadCategories để lấy danh sách Categories
            var allProducts = db.Products.ToList();
            return View(allProducts);
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
