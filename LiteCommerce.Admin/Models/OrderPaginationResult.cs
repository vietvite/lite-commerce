using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.Models
{
    public class OrderPaginationResult : PaginationResult
    {
        public List<Order> Data { get; set; }
        public string selectedCountry { get; set; }
        public string selectedCategory { get; set; }
        public string selectedEmployee { get; set; }
        public string selectedShipper { get; set; }
    }
}