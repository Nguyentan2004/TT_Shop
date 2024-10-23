using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
            // Assuming you want to pass the cart items to the view
            List<CartItem> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            return View(cart);
        }


        // Action to add product to cart
        public ActionResult AddToCart(int product_id)
        {
            if (Session["user_id"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userId = (int)Session["user_id"];

            // Ensure the product exists in the database before proceeding
            var product = db.Products.Find(product_id);
            if (product == null)
            {
                return HttpNotFound("Product not found.");
            }

            // Retrieve the cart from session or create a new one if it doesn't exist
            List<CartItem> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(c => c.IdSanPham == product_id);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    UserId = userId,
                    IdSanPham = product_id,
                    SoLuong = 1,
                    TenSanPham = product.name, // Lấy tên sản phẩm từ product
                    Gia = product.price, // Lấy giá sản phẩm từ product
                    ImageUrl = product.image_url // Lấy URL hình ảnh từ product
                };
                cart.Add(cartItem);
            }
            else
            {
                cartItem.SoLuong++;
            }

            // Save the cart back to the session
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

            // Ensure the product exists in the database before proceeding
            var product = db.Products.Find(product_id);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            // Retrieve the cart from session or create a new one if it doesn't exist
            List<CartItem> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(c => c.IdSanPham == product_id);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    UserId = userId,
                    IdSanPham = product_id,
                    SoLuong = 1,
                    TenSanPham = product.name, // Lấy tên sản phẩm từ product
                    Gia = product.price, // Lấy giá sản phẩm từ product
                    ImageUrl = product.image_url // Lấy URL hình ảnh từ product
                };
                cart.Add(cartItem);
            }
            else
            {
                cartItem.SoLuong++;
            }

            // Save the cart back to the session
            Session["Cart"] = cart;

            return Json(new { success = true, message = "Product added to cart.", cartItemCount = cart.Count });
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int cartItemId, int quantity)
        {
            int userId = (int)Session["user_id"];
            var cartItem = db.CartItems.FirstOrDefault(ci => ci.IdSanPham == cartItemId && ci.UserId == userId);
            if (cartItem != null)
            {
                cartItem.SoLuong = quantity;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Item not found" });
        }



    }


}
