using System.Collections.Generic;
using System;

namespace LiteCommerce.DomainModels
{
    public class OrderPostRequest
    {
        public int OrderID { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public decimal Freight { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipCountry { get; set; }

        public string ShipperID { get; set; }
        public string CustomerID { get; set; }
        public string EmployeeID { get; set; }

        public int OrderProduct { get; set; }
        public int UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
    }
}