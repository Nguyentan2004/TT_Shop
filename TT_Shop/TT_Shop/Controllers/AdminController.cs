using System.Web.Mvc;

namespace TTshop.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // Action to navigate to AccountsView
        public ActionResult AccountsView()
        {
            return View("~/Views/AccountManagement/AccountsView.cshtml");
        }

        // Action to navigate to ProductsView
        public ActionResult ProductsView()
        {
            return View("~/Views/ProductManagement/ProductsView.cshtml");
        }

        // Action to navigate to ReportsView
        public ActionResult ReportsView()
        {
            return View("~/Views/Report/ReportsView.cshtml");
        }

        // Action to navigate to TransactionHistoryView
        public ActionResult TransactionHistoryView()
        {
            return View("~/Views/TransactionHistory/TransactionHistoryView.cshtml");
        }
    }
}