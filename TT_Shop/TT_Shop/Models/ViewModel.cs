﻿using System;
using System.Collections.Generic;
using TT_Shop.Models; // Ensure this namespace includes your EF models

namespace TT_Shop.Models
{
    public class OrdersReportViewModel
    {
        public OrdersReportViewModel()
        {
            MonthlyIncome = new Dictionary<string, MonthlyData>();
            SalesDetails = new List<SalesDetail>();
            Orders = new List<Order>(); // Use the existing Order entity
        }

        public DateTime SelectedDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal DailyIncome { get; set; }
        public Dictionary<string, MonthlyData> MonthlyIncome { get; set; }
        public int TotalProductsSold { get; set; }
        public List<SalesDetail> SalesDetails { get; set; }
        public List<Order> Orders { get; set; } // Use the existing Order entity

        public class SalesDetail
        {
            public string ProductName { get; set; }
            public int QuantitySold { get; set; }
        }

        public class MonthlyData
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal TotalIncome { get; set; }
            public int TotalProductsSold { get; set; }
        }

        public class Order
        {
            public int OrderId { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string CustomerName { get; set; }
        }
    }
}
