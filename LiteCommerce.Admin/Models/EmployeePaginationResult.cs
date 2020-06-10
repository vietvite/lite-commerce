using LiteCommerce.DomainModels;
using System.Collections.Generic;

namespace LiteCommerce.Models
{
    public class EmployeePaginationResult : PaginationResult
    {
        public List<Employee> Data { get; set; }
    }
}