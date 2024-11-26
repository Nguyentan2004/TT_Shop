using System;
using System.Collections.Generic;
using TT_Shop.Models; // Ensure this namespace includes your EF models

namespace TT_Shop.Models
{
    public class OrdersReportViewModel
    {
        public OrdersReportViewModel()
        {
            MonthlyIncome = new Dictionary<string, decimal>();
            SalesDetails = new List<SalesDetail>();
            Orders = new List<Order>(); // Use the existing Order entity
        }

        public DateTime SelectedDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal DailyIncome { get; set; }
        public Dictionary<string, decimal> MonthlyIncome { get; set; }
        public int TotalProductsSold { get; set; }
        public List<SalesDetail> SalesDetails { get; set; }
        public List<Order> Orders { get; set; } // Use the existing Order entity

        public class SalesDetail
        {
            public string ProductName { get; set; }
            public int QuantitySold { get; set; }
        }
    }
}