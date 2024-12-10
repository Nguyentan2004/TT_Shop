using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TT_Shop.Models;

namespace TTshop.Controllers
{
    public class OrdersController : Controller
    {
        private QLTTShopEntities db = new QLTTShopEntities();

        public async Task<ActionResult> Index(int page = 1, int pageSize = 15)
        {
            var orders = db.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.order_date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            int totalOrders = await db.Orders.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalOrders / pageSize);
            ViewBag.CurrentPage = page;

            return View(await orders.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders
                .Include(o => o.Order_Details.Select(od => od.Product))
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.order_id == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.Users, "user_id", "fullname");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "order_id,user_id,order_status,total_amount,shipping_address,order_date,updated_at")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.user_id = new SelectList(db.Users, "user_id", "fullname", order.user_id);
            return View(order);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.user_id = new SelectList(db.Users, "user_id", "fullname", order.user_id);
            ViewBag.StatusOptions = new SelectList(new List<string> { "Chưa giải quyết", "Đã duyệt", "Đã hủy" });
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "order_id,user_id,order_status,total_amount,shipping_address,order_date,updated_at")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.user_id = new SelectList(db.Users, "user_id", "fullname", order.user_id);
            ViewBag.StatusOptions = new SelectList(new List<string> { "Chưa giải quyết", "Đã duyệt", "Đã hủy" });
            return View(order);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            var orderDetails = db.Order_Details.Where(od => od.order_id == id);

            db.Order_Details.RemoveRange(orderDetails);

            db.Orders.Remove(order);

            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpPost]
        public ActionResult ApproveOrder(int id)
        {
            var order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.order_status = "Approved";
            order.updated_at = DateTime.Now;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}