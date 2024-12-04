using System;

namespace TT_Shop.Models
{
    public class ReviewViewModel
    {
        public string userName { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }
        public DateTime createdAt { get; set; }
    }
}
