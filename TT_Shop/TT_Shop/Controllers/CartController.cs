using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TT_Shop.Models;

namespace TT_Shop.Controllers
{
    public class CartController : Controller
    {
        private QLTTShopEntities db = new QLTTShopEntities();

        // GET: Cart
        public ActionResult Index()
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            int orderId = GenerateOrderId();
            ViewBag.OrderId = orderId;

            // Calculate total price
            decimal totalPrice = cart.Sum(item => item.Gia.GetValueOrDefault() * item.SoLuong.GetValueOrDefault());
            ViewBag.TotalPrice = totalPrice;

            return View(cart);
        }


        private int GenerateOrderId()
        {
            return new Random().Next(1000, 9999);
        }

        public ActionResult AddToCart(int product_id)
        {
            if (Session["user_id"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userId = (int)Session["user_id"];
            var product = db.Products.Find(product_id);
            if (product == null)
            {
                return HttpNotFound("Product not found.");
            }

            List<CartItem> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.IdSanPham == product_id);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    UserId = userId,
                    IdSanPham = product_id,
                    SoLuong = 1,
                    TenSanPham = product.name,
                    Gia = product.price,
                    ImageUrl = product.image_url
                };
                cart.Add(cartItem);
            }
            else
            {
                cartItem.SoLuong++;
            }

            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult AddToCartAjax(int product_id)
        {
            if (Session["user_id"] == null)
            {
                return Json(new { success = false, message = "User not logged in." });
            }

            int userId = (int)Session["user_id"];
            var product = db.Products.Find(product_id);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            List<CartItem> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.IdSanPham == product_id);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    UserId = userId,
                    IdSanPham = product_id,
                    SoLuong = 1,
                    TenSanPham = product.name,
                    Gia = product.price,
                    ImageUrl = product.image_url
                };
                cart.Add(cartItem);
            }
            else
            {
                cartItem.SoLuong++;
            }

            Session["Cart"] = cart;
            return Json(new { success = true, message = "Product added to cart.", cartItemCount = cart.Count });
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int cartItemId, int quantity)
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.IdSanPham == cartItemId);
            if (cartItem != null)
            {
                cartItem.SoLuong = quantity;
                Session["Cart"] = cart;
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Item not found" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaGioHang(int iMaSach)
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            var sanpham = cart.SingleOrDefault(n => n.IdSanPham == iMaSach);
            if (sanpham != null)
            {
                cart.Remove(sanpham);
                Session["Cart"] = cart;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult CalculateTotal(List<int> selectedProductIds)
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            decimal total = 0;

            foreach (var id in selectedProductIds)
            {
                var cartItem = cart.FirstOrDefault(c => c.IdSanPham == id);
                if (cartItem != null)
                {
                    total += cartItem.Gia.GetValueOrDefault() * cartItem.SoLuong.GetValueOrDefault();
                }
            }

            return Json(new { total });
        }
    }
}
