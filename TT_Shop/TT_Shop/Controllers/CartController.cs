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
        //public ActionResult Index()
        //{
        //    // Lấy các mục trong giỏ hàng từ cơ sở dữ liệu
        //    List<CartItem> cartItems = db.CartItems.ToList();
        //    return View(cartItems);
        //}
    }
}
