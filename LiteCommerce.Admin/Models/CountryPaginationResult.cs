using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.Models
{
    public class CountryPaginationResult : PaginationResult
    {
        public List<Country> Data { get; set; }

    }
}