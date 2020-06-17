using System.Collections.Generic;
using System;

namespace LiteCommerce.DomainModels
{
    public class OrderDetails : Order
    {
        // Order details
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        virtual public Product Product { get; set; }
    }
}