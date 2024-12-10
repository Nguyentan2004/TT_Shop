using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TT_Shop.Models;
using static TT_Shop.Models.OrdersReportViewModel;

namespace TTshop.Controllers
{
    public class ReportController : Controller
    {
        private QLTTShopEntities db = new QLTTShopEntities();

        // GET: Report
        public ActionResult ReportsView(int page = 1, int pageSize = 10)
        {
            var today = DateTime.Now.Date;
            var totalOrders = db.Orders.Count(o => DbFunctions.TruncateTime(o.order_date) == today);
            var dailyIncome = db.Orders.Where(o => DbFunctions.TruncateTime(o.order_date) == today).Sum(o => (decimal?)o.total_amount) ?? 0;

            var monthlyData = db.Orders
                .GroupBy(o => new { Year = DbFunctions.TruncateTime(o.order_date).Value.Year, Month = DbFunctions.TruncateTime(o.order_date).Value.Month })
                .Select(g => new OrdersReportViewModel.MonthlyData
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalIncome = g.Sum(o => (decimal?)o.total_amount) ?? 0,
                    TotalProductsSold = g.SelectMany(o => o.Order_Details).Sum(od => (int?)od.quantity) ?? 0
                })
                .ToList();

            var monthlyIncome = monthlyData.ToDictionary(
                x => $"{x.Year}-{x.Month:00}",
                x => x);

            var orders = db.Orders
                .Where(o => DbFunctions.TruncateTime(o.order_date) == today)
                .OrderBy(o => o.order_date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new OrdersReportViewModel
            {
                SelectedDate = today,
                TotalOrders = totalOrders,
                DailyIncome = dailyIncome,
                MonthlyIncome = monthlyIncome,
                TotalProductsSold = monthlyData.Sum(x => x.TotalProductsSold),
                Orders = orders.Select(o => new OrdersReportViewModel.Order
                {
                    OrderId = o.order_id,
                    OrderDate = (DateTime)o.order_date,
                    TotalAmount = (decimal)o.total_amount,
                    CustomerName = o.User.username
                }).ToList()
            };

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GenerateReport(DateTime selectedDate, int page = 1, int pageSize = 10)
        {
            var orders = db.Orders
                .Where(o => DbFunctions.TruncateTime(o.order_date) == selectedDate.Date)
                .Include(o => o.Order_Details)
                .Include(o => o.User)
                .OrderBy(o => o.order_date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalOrders = db.Orders.Count(o => DbFunctions.TruncateTime(o.order_date) == selectedDate.Date);
            var dailyIncome = orders.Sum(o => o.total_amount);

            var monthlyData = db.Orders
                .GroupBy(o => new { Year = DbFunctions.TruncateTime(o.order_date).Value.Year, Month = DbFunctions.TruncateTime(o.order_date).Value.Month })
                .Select(g => new OrdersReportViewModel.MonthlyData
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalIncome = g.Sum(o => (decimal?)o.total_amount) ?? 0,
                    TotalProductsSold = g.SelectMany(o => o.Order_Details).Sum(od => (int?)od.quantity) ?? 0
                })
                .ToList();

            var monthlyIncome = monthlyData.ToDictionary(
                x => $"{x.Year}-{x.Month:00}",
                x => x);

            var viewModel = new OrdersReportViewModel
            {
                SelectedDate = selectedDate,
                TotalOrders = totalOrders,
                DailyIncome = (decimal)dailyIncome,
                Orders = orders.Select(o => new OrdersReportViewModel.Order
                {
                    OrderId = o.order_id,
                    OrderDate = (DateTime)o.order_date,
                    TotalAmount = (decimal)o.total_amount,
                    CustomerName = o.User.username
                }).ToList(),
                MonthlyIncome = monthlyIncome,
                TotalProductsSold = monthlyData.Sum(x => x.TotalProductsSold)
            };

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

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
        [HttpPost]
        public ActionResult _SalesDetails(string month)
        {
            // Lấy dữ liệu chi tiết bán hàng cho tháng được chọn
            var salesDetails = GetSalesDetailsForMonth(month);

            // Tạo mô hình dữ liệu
            var model = new OrdersReportViewModel
            {
                SalesDetails = salesDetails
            };

            // Trả về PartialView với mô hình dữ liệu
            return PartialView("_SalesDetails", model);
        }

        private List<SalesDetail> GetSalesDetailsForMonth(string month)
        {
            // Implement logic to retrieve sales details for the specified month
            // This is just an example
            return new List<SalesDetail>
        {
            new SalesDetail { ProductName = "Product 1", QuantitySold = 10 },
            new SalesDetail { ProductName = "Product 2", QuantitySold = 5 }
        };
        }

        //private ActionResult ViewMonth()
        //{
        //    return View();
        //}
        public ActionResult ViewMonth(string month, int page = 1, int pageSize = 10)
        {
            var yearMonth = month.Split('-');
            int year = int.Parse(yearMonth[0]);
            int monthNumber = int.Parse(yearMonth[1]);

            var orders = db.Orders
                .Where(o => DbFunctions.TruncateTime(o.order_date).Value.Year == year && DbFunctions.TruncateTime(o.order_date).Value.Month == monthNumber)
                .Include(o => o.Order_Details)
                .Include(o => o.User)
                .OrderBy(o => o.order_date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)db.Orders.Count(o => DbFunctions.TruncateTime(o.order_date).Value.Year == year && DbFunctions.TruncateTime(o.order_date).Value.Month == monthNumber) / pageSize);
            ViewBag.SelectedMonth = month;

            return View(orders);
        }
    }
}