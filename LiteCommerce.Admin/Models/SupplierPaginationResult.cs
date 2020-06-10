using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.Models
{
    public class SupplierPaginationResult : PaginationResult
    {
        public List<Supplier> Data { get; set; }
    }
}