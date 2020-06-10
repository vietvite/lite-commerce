using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.Models
{
    public class ProductPaginationResult : PaginationResult
    {
        public List<Product> Data { get; set; }

    }
}