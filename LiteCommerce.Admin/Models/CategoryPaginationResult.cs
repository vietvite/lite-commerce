using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.Models
{
    public class CategoryPaginationResult : PaginationResult
    {
        public List<Category> Data { get; set; }

    }
}