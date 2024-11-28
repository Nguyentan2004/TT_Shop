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
            }
            return Json(new { success = true, message = "Product removed from cart." }, JsonRequestBehavior.AllowGet);
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
            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Complete()
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            // Create a new order
            var order = new Order
            {
                user_id = (int)Session["user_id"], // Get the current user ID
                order_status = "Pending",
                total_amount = cart.Sum(item => item.Gia.GetValueOrDefault() * item.SoLuong.GetValueOrDefault()),
                order_date = DateTime.Now,
                shipping_address = "Your shipping address", // Get the shipping address
                updated_at = DateTime.Now
            };

            db.Orders.Add(order);
            db.SaveChanges();

            // Create order details for each cart item
            foreach (var item in cart)
            {
                var orderDetail = new Order_Details
                {
                    order_id = order.order_id,
                    product_id = item.IdSanPham,
                    quantity = item.SoLuong,
                    price = item.Gia
                };

                db.Order_Details.Add(orderDetail);
            }

            db.SaveChanges();

            // Create a payment record
            var payment = new Payment
            {
                order_id = order.order_id,
                payment_method = "Banking", // Ensure this value matches the allowed values in the database
                payment_status = "Completed",
                payment_date = DateTime.Now
            };

            db.Payments.Add(payment);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Log the inner exception message
                var innerException = ex.InnerException?.InnerException?.Message;
                // Handle the exception as needed
            }

            // Clear the cart
            Session["Cart"] = null;

            return View();
        }


        private IEnumerable<CartItem> GetCartItems()
        {
            // Replace with your logic to get cart items from the session or database
            return Session["Cart"] as List<CartItem> ?? new List<CartItem>();
        }

        public ActionResult Category(int id, int page = 1)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            // Lấy tổng số sản phẩm trong danh mục
            int pageSize = 9;  // Số sản phẩm hiển thị mỗi trang
            int totalProducts = db.Products.Count(p => p.category_id == id);

            // Tính tổng số trang
            int totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            // Lấy danh sách sản phẩm theo trang
            var products = db.Products
                             .Where(p => p.category_id == id)
                             .OrderBy(p => p.product_id)  // Sắp xếp theo ID sản phẩm hoặc theo một thuộc tính khác
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            // Truyền dữ liệu sang view
            ViewBag.CategoryName = category.name;
            ViewBag.CategoryId = id;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(products);
        }

    }
}