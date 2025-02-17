﻿using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TT_Shop.Models;

namespace TT_Shop.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        private QLTTShopEntities db = new QLTTShopEntities();
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(string TenDN, string Matkhau)
        {
            if (string.IsNullOrEmpty(TenDN))
            {
                ViewData["Loi1"] = "Tên đăng nhập không được để trống";
            }
            if (string.IsNullOrEmpty(Matkhau))
            {
                ViewData["Loi2"] = "Mật khẩu không được để trống";
            }

            if (!string.IsNullOrEmpty(TenDN) && !string.IsNullOrEmpty(Matkhau))
            {
                var user = db.Users.FirstOrDefault(u => u.username == TenDN && u.password == Matkhau);

                if (user != null)
                {
                    Session["User"] = user; 
                    Session["user_id"] = user.user_id; 
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng!";
                }
            }
            return View();
        }


        public ActionResult Logout()
        {
            Session["User"] = null;
            Session["user_id"] = null; 
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Details(int page = 1, int PageSize =10)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("login", "User");
            }

            var khachHang = Session["User"] as User;

            var orders = db.Orders.Where(o => o.user_id == khachHang.user_id)
            .OrderBy(o => o.order_id)
                                  .Skip((page - 1) * PageSize)
                                  .Take(PageSize)
                                  .ToList();

            int totalOrders = db.Orders.Count(o => o.user_id == khachHang.user_id);
            int totalPages = (int)Math.Ceiling((double)totalOrders / PageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            khachHang.Orders = orders;

            return View(khachHang);
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(User user, string confirmPassword)
        {
            if (string.IsNullOrEmpty(user.fullname))
            {
                ViewData["LoiName"] = "Họ và Tên không được để trống";
            }
            if (string.IsNullOrEmpty(user.username))
            {
                ViewData["Loi1"] = "Tên đăng nhập không được để trống";
            }
            if (string.IsNullOrEmpty(user.password))
            {
                ViewData["Loi2"] = "Mật khẩu không được để trống";
            }
            else if (user.password.Length <= 6)
            {
                ViewData["Loi2"] = "Mật khẩu phải có ít nhất 7 ký tự";
            }
            if (string.IsNullOrEmpty(confirmPassword))
            {
                ViewData["Loi3"] = "Vui lòng nhập lại mật khẩu";
            }
            if (string.IsNullOrEmpty(user.email))
            {
                ViewData["Loi4"] = "Email không được để trống";
            }

            if (user.password != confirmPassword)
            {
                ViewData["Loi3"] = "Mật khẩu và mật khẩu xác nhận không khớp";
            }

            if (string.IsNullOrEmpty(ViewData["LoiName"] as string) &&
                string.IsNullOrEmpty(ViewData["Loi1"] as string) &&
                string.IsNullOrEmpty(ViewData["Loi2"] as string) &&
                string.IsNullOrEmpty(ViewData["Loi3"] as string) &&
                string.IsNullOrEmpty(ViewData["Loi4"] as string))
            {
                var existingUser = db.Users.FirstOrDefault(u => u.username == user.username || u.email == user.email);
                if (existingUser != null)
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc email đã tồn tại!";
                    return View();
                }

                user.role = "customer";

                try
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine("Property: {0} Error: {1}",
                                validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }

                    ViewBag.Thongbao = "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng thử lại.";
                    return View();
                }
            }

            return View();
        }

        public ActionResult OrderDetails(int id)
        {
            var order = db.Orders.Include("Order_Details").Include("Order_Details.Product").FirstOrDefault(o => o.order_id == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
        public ActionResult ForgotPassword()
        {

            return View();

        }
        [HttpPost]
        public ActionResult ForgotPassword(string identifier)
        {

            var user = (from u in db.Users
                        where identifier == u.email || identifier == u.username
                        select u).FirstOrDefault();

            if (user != null)
            {
                ViewBag.Username = user.username;
                ViewBag.Fullname = user.fullname;
                ViewBag.Password = user.password;
            }
            else
            {
                ViewBag.ErrorMessage = "Không tìm thấy tài khoản với thông tin đã cung cấp.";
            }

            return View();

        }

    }
}
