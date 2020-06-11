using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.Models
{
    public class AttributePaginationResult : PaginationResult
    {
        public List<LiteCommerce.DomainModels.Attribute> Data { get; set; }
        public string CategoryID { get; set; }

    }
}