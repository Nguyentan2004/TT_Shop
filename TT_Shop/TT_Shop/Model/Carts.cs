using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TT_Shop.Model
{
    public class Carts
    {
        public int cart_id { get; set; }
        public int user_id { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
     

    }
}