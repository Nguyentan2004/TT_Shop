using System;
using System.Collections.Generic;
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
                // Kiểm tra người dùng trong cơ sở dữ liệu
                var user = db.Users.FirstOrDefault(u => u.username == TenDN && u.password == Matkhau);

                if (user != null)
                {
                    // Nếu thông tin đăng nhập hợp lệ, lưu thông tin người dùng vào session
                    Session["User"] = user; // Store the entire user object
                    Session["user_id"] = user.user_id; // Lưu user_id vào session
                                                       // Chuyển hướng đến trang chủ hoặc trang dashboard
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
            Session["user_id"] = null; // Xóa user_id khỏi session
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Details()
        {

            if (Session["User"] == null)
            {
                return RedirectToAction("login", "User");
            }
            var khachHang = Session["User"] as User;
            return View(khachHang);

        }


        public ActionResult Register()
        {
            return View();
        }

        // POST: Nguoidung/Register
        [HttpPost]
        public ActionResult Register(string username, string password, string confirmPassword, string email)
        {
            // Kiểm tra các trường username, password, email có bị bỏ trống không
            if (string.IsNullOrEmpty(username))
            {
                ViewData["Loi1"] = "Tên đăng nhập không được để trống";
            }
            if (string.IsNullOrEmpty(password))
            {
                ViewData["Loi2"] = "Mật khẩu không được để trống";
            }
            if (string.IsNullOrEmpty(confirmPassword))
            {
                ViewData["Loi3"] = "Vui lòng nhập lại mật khẩu";
            }
            if (string.IsNullOrEmpty(email))
            {
                ViewData["Loi4"] = "Email không được để trống";
            }

            // Kiểm tra xem mật khẩu và mật khẩu xác nhận có khớp nhau không
            if (password != confirmPassword)
            {
                ViewData["Loi3"] = "Mật khẩu và mật khẩu xác nhận không khớp";
            }

            // Kiểm tra nếu không có lỗi
            if (string.IsNullOrEmpty(ViewData["Loi1"] as string) &&
                string.IsNullOrEmpty(ViewData["Loi2"] as string) &&
                string.IsNullOrEmpty(ViewData["Loi3"] as string) &&
                string.IsNullOrEmpty(ViewData["Loi4"] as string))
            {
                // Kiểm tra tên đăng nhập và email đã tồn tại chưa
                var existingUser = db.Users.FirstOrDefault(u => u.username == username || u.email == email);
                if (existingUser != null)
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc email đã tồn tại!";
                    return View();
                }

                // Tạo người dùng mới
                var newUser = new User
                {
                    username = username,
                    password = password,
                    email = email
                };

                try
                {
                    // Lưu người dùng mới vào cơ sở dữ liệu
                    db.Users.Add(newUser);
                    db.SaveChanges();

                    // Chuyển hướng tới trang đăng nhập sau khi đăng ký thành công
                    return RedirectToAction("Login");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    // Bắt lỗi xác thực và hiển thị chi tiết lỗi
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
    }
}
