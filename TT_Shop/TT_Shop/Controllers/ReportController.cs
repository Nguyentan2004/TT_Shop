using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TT_Shop.Models;

namespace TTshop.Controllers
{
    public class ReportController : Controller
    {
        private QLTTShopEntities db = new QLTTShopEntities();

        // GET: Report
        public ActionResult ReportsView()
        {
            var today = DateTime.Now.Date;
            var totalOrders = db.Orders.Count(o => DbFunctions.TruncateTime(o.order_date) == today);
            var dailyIncome = db.Orders.Where(o => DbFunctions.TruncateTime(o.order_date) == today).Sum(o => (decimal?)o.total_amount) ?? 0;

            var monthlyIncome = db.Orders
                .GroupBy(o => new { Year = DbFunctions.TruncateTime(o.order_date).Value.Year, Month = DbFunctions.TruncateTime(o.order_date).Value.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalIncome = g.Sum(o => (decimal?)o.total_amount) ?? 0
                })
                .ToList()
                .ToDictionary(x => $"{x.Year}-{x.Month:00}", x => x.TotalIncome);

            var totalProductsSold = db.Order_Details.Sum(od => (int?)od.quantity) ?? 0;

            var viewModel = new OrdersReportViewModel
            {
                SelectedDate = today,
                TotalOrders = totalOrders,
                DailyIncome = dailyIncome,
                MonthlyIncome = monthlyIncome,
                TotalProductsSold = totalProductsSold
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GenerateReport(DateTime selectedDate)
        {
            var orders = db.Orders
                .Where(o => DbFunctions.TruncateTime(o.order_date) == selectedDate.Date)
                .Include(o => o.Order_Details)
                .Include(o => o.User)
                .ToList();

            var totalOrders = orders.Count;
            var dailyIncome = orders.Sum(o => o.total_amount);

            var viewModel = new OrdersReportViewModel
            {
                SelectedDate = selectedDate,
                TotalOrders = totalOrders,
                DailyIncome = (decimal)dailyIncome,
                Orders = orders,
                MonthlyIncome = db.Orders
                    .GroupBy(o => new { Year = DbFunctions.TruncateTime(o.order_date).Value.Year, Month = DbFunctions.TruncateTime(o.order_date).Value.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        TotalIncome = g.Sum(o => (decimal?)o.total_amount) ?? 0
                    })
                    .ToList()
                    .ToDictionary(x => $"{x.Year}-{x.Month:00}", x => x.TotalIncome),
                TotalProductsSold = db.Order_Details.Sum(od => (int?)od.quantity) ?? 0
            };

            return View("ReportsView", viewModel);
        }

        [HttpPost]
        public ActionResult ViewSalesDetails(string month)
        {
            var yearMonth = month.Split('-');
            int year = int.Parse(yearMonth[0]);
            int monthNumber = int.Parse(yearMonth[1]);

            var salesDetails = db.Order_Details
                .Where(od => DbFunctions.TruncateTime(od.Order.order_date).Value.Year == year && DbFunctions.TruncateTime(od.Order.order_date).Value.Month == monthNumber)
                .GroupBy(od => od.Product.name)
                .Select(g => new OrdersReportViewModel.SalesDetail
                {
                    ProductName = g.Key,
                    QuantitySold = (int)g.Sum(od => od.quantity)
                })
                .ToList();

            var viewModel = new OrdersReportViewModel
            {
                SalesDetails = salesDetails
            };

            return PartialView("_SalesDetails", viewModel);
        }
    }
}