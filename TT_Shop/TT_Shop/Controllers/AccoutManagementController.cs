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
    public class AccoutManagementController : Controller
    {
        private QLTTShopEntities db = new QLTTShopEntities();

     
        public async Task<ActionResult> Index()
        {
            var users = db.Users.Include(u => u.CartItem);
            return View(await users.ToListAsync());
        }

     
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

     
        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.CartItems, "UserId", "TenSanPham");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "user_id,fullname,username,email,password,role,created_at,updated_at")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.user_id = new SelectList(db.CartItems, "UserId", "TenSanPham", user.user_id);
            return View(user);
        }

       
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.user_id = new SelectList(db.CartItems, "UserId", "TenSanPham", user.user_id);
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "user_id,fullname,username,email,password,role,created_at")] User user)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the existing user from the database
                var existingUser = await db.Users.FindAsync(user.user_id);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                // Preserve the original created_at value
                user.created_at = existingUser.created_at;

                // Set the updated_at field to the current date and time
                user.updated_at = DateTime.Now;

                // Update the user entity
                db.Entry(existingUser).CurrentValues.SetValues(user);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.user_id = new SelectList(db.CartItems, "UserId", "TenSanPham", user.user_id);
            return View(user);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            User user = await db.Users.FindAsync(id);
            db.Users.Remove(user);
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
    }
}
