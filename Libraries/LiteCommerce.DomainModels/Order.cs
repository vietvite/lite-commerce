using System.Collections.Generic;
using System;

namespace LiteCommerce.DomainModels
{
    public class Order
    {
        public int OrderID { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public decimal Freight { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipCountry { get; set; }

        virtual public Shipper Shipper { get; set; }
        virtual public Customer Customer { get; set; }
        virtual public Employee Employee { get; set; }
    }
}