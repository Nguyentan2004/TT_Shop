using System.Web.Mvc;

namespace TTshop.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AccountsView()
        {
            return View("~/Views/AccountManagement/AccountsView.cshtml");
        }

        public ActionResult ProductsView()
        {
            return View("~/Views/ProductManagement/ProductsView.cshtml");
        }

        public ActionResult ReportsView()
        {
            return View("~/Views/Report/ReportsView.cshtml");
        }

        public ActionResult TransactionHistoryView()
        {
            return View("~/Views/TransactionHistory/TransactionHistoryView.cshtml");
        }
    }
}