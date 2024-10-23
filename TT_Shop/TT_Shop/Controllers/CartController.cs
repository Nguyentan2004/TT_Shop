using System;
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
            if (Session["user_id"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userId = (int)Session["user_id"];
            var cartItems = db.CartItems.Where(c => c.UserId == userId).ToList();
            return View(cartItems);
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

            var cartItem = db.CartItems.FirstOrDefault(c => c.UserId == userId && c.IdSanPham == product_id);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    UserId = userId,
                    IdSanPham = product_id,
                    SoLuong = 1,
                    TenSanPham = product.name, // Ensure this field is populated
                    Gia = product.price, // Ensure this field is populated
                    ImageUrl = product.image_url // Ensure this field is populated
                };
                db.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.SoLuong++;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        var errorMessage = $"Entity: {validationErrors.Entry.Entity.GetType().Name}, " +
                                           $"Property: {validationError.PropertyName}, " +
                                           $"Error: {validationError.ErrorMessage}";

                        System.Diagnostics.Debug.WriteLine(errorMessage);
                    }
                }

                // Optionally, return an error view with more context
                return new HttpStatusCodeResult(500, "Validation failed. Please check input values.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"An error occurred: {ex.Message}");
                return new HttpStatusCodeResult(500, "An internal error occurred. Please try again later.");
            }

            return RedirectToAction("Index");
        }
    }
}
