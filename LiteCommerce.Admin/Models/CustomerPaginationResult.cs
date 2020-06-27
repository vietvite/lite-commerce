using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.Models
{
    public class CustomerPaginationResult : PaginationResult
    {
        public string SelectedCountry { get; set; }
        public List<Customer> Data { get; set; }
    }
}