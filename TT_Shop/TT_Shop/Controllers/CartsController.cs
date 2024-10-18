using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TT_Shop.Model;

namespace TT_Shop.Controllers
{
    public class CartsController : Controller
    {
        private QLTTShopEntities db = new QLTTShopEntities();

        // GET: Carts
        public ActionResult Index()
        {
            var cart = Session["Cart"] as List<Carts> ?? new List<Carts>();
            return View(cart);
        }

        // POST: Carts/AddToCart
        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity)
        {
            var product = db.Products.Find(productId);
            if (product == null)
            {
                return HttpNotFound();
            }

            var cart = Session["Cart"] as List<Carts> ?? new List<Carts>();

            var cartItem = cart.FirstOrDefault(c => c.product_id == productId);
            if (cartItem != null)
            {
                cartItem.quantity += quantity;
            }
            else
            {
                cart.Add(new Carts
                {
                    product_id = productId,
                    quantity = quantity,
                    price = (int)product.price,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                });
            }

            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        // GET: Carts/Checkout
        public ActionResult Checkout()
        {
            var cart = Session["Cart"] as List<Carts> ?? new List<Carts>();
            return View(cart);
        }

        // POST: Carts/ProcessPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessPayment(string shippingAddress)
        {
            var userId = 1; // Replace with actual user ID
            var cart = Session["Cart"] as List<Carts> ?? new List<Carts>();

            if (cart.Count == 0)
            {
                return RedirectToAction("Index");
            }

            var order = new Order
            {
                user_id = userId,
                order_status = "Pending",
                total_amount = cart.Sum(c => c.price * c.quantity),
                shipping_address = shippingAddress,
                order_date = DateTime.Now,
                updated_at = DateTime.Now
            };

            db.Orders.Add(order);
            db.SaveChanges();

            foreach (var item in cart)
            {
                var orderDetail = new Order_Details
                {
                    order_id = order.order_id,
                    product_id = item.product_id,
                    quantity = item.quantity,
                    price = item.price
                };

                db.Order_Details.Add(orderDetail);
            }

            db.SaveChanges();

            // Clear the cart
            Session["Cart"] = null;

            return RedirectToAction("Index", "Orders");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}