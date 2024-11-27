using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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

            // Tính tổng giá tiền của các sản phẩm trong giỏ hàng
            decimal totalPrice = cart.Sum(item => item.Gia.GetValueOrDefault() * item.SoLuong.GetValueOrDefault());
            ViewBag.TotalPrice = totalPrice;

            // Lấy danh sách sản phẩm đã chọn từ Session
            List<int> selectedProducts = Session["SelectedProducts"] as List<int> ?? new List<int>();
            ViewBag.SelectedProducts = selectedProducts;

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

        [HttpGet]
        public ActionResult AddToCartAjax(int product_id)
        {
            if (Session["user_id"] == null)
            {
                return Json(new { success = false, message = "Please log in to add products to cart." }, JsonRequestBehavior.AllowGet);
            }

            int userId = (int)Session["user_id"];
            var product = db.Products.Find(product_id);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." }, JsonRequestBehavior.AllowGet);
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
            int totalQuantity = cart.Sum(item => item.SoLuong.GetValueOrDefault());

            return Json(new { success = true, message = "Thêm vào giỏ hàng thành công.", totalQuantity = totalQuantity }, JsonRequestBehavior.AllowGet);
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

                // Calculate the updated total price
                decimal totalPrice = cart.Sum(item => item.Gia.GetValueOrDefault() * item.SoLuong.GetValueOrDefault());

                return Json(new { success = true, totalPrice = totalPrice.ToString("N0") });
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
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công.";
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaHetGioHang()
        {
            Session["Cart"] = new List<CartItem>();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SaveSelectedProducts(List<int> selectedProductIds)
        {
            Session["SelectedProducts"] = selectedProductIds;
            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult GetSelectedProducts()
        {
            var selectedProductIds = Session["SelectedProducts"] as List<int> ?? new List<int>();
            return Json(new { success = true, selectedProductIds = selectedProductIds }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Payment()
        {
            var cartItems = GetCartItems();
            if (cartItems == null || !cartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }
            Session["Cart"] = cartItems;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Complete()
        {
            try
            {
                var cartItems = Session["Cart"] as List<CartItem>;
                if (cartItems == null || !cartItems.Any())
                {
                    TempData["ErrorMessage"] = "Your cart is empty.";
                    return RedirectToAction("Index");
                }

                // Process the payment and create the order
                var orderId = GenerateOrderId();
                var order = new Order
                {
                    order_id = orderId,
                    user_id = 1, // Replace with actual user ID
                    order_status = "Pending",
                    total_amount = cartItems.Sum(i => i.Gia.GetValueOrDefault() * i.SoLuong.GetValueOrDefault()),
                    order_date = DateTime.Now
                };
                db.Orders.Add(order);
                db.SaveChanges();

                foreach (var item in cartItems)
                {
                    var orderDetail = new Order_Details
                    {
                        order_id = orderId,
                        product_id = item.IdSanPham,
                        quantity = item.SoLuong,
                        price = item.Gia
                    };
                    db.Order_Details.Add(orderDetail);
                }
                db.SaveChanges();

                // Clear the cart
                Session["Cart"] = null;
                TempData["SuccessMessage"] = "Payment completed successfully.";
                return RedirectToAction("Complete");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request: " + ex.Message;
                return RedirectToAction("Index");
            }
        }


        private IEnumerable<CartItem> GetCartItems()
        {
            // Replace with your logic to get cart items from the session or database
            return Session["Cart"] as List<CartItem> ?? new List<CartItem>();
        }


    }
}
