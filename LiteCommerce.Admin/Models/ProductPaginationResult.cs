using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.Models
{
    public class ProductPaginationResult : PaginationResult
    {
        public List<Product> Data { get; set; }
        public string SelectedCategory { get; set; }
        public string SelectedSupplier { get; set; }
    }
}