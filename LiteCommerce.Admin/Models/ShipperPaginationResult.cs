using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.Models
{
    public class ShipperPaginationResult : PaginationResult
    {
        public List<Shipper> Data { get; set; }
    }
}