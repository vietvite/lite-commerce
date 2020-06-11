using System;
using Microsoft.AspNetCore.Http;

namespace LiteCommerce.Models
{
    public class ProductPostRequest
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int SupplierID { get; set; }
        public int CategoryID { get; set; }
        public string QuantityPerUnit { get; set; }
        public int UnitPrice { get; set; }
        public string Descriptions { get; set; }
        public IFormFile PhotoPath { get; set; }
    }
}