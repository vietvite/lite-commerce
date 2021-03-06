using System;
using Microsoft.AspNetCore.Http;

namespace LiteCommerce.Models
{
    public class ProfilePostRequest
    {
        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string HomePhone { get; set; }
        public IFormFile PhotoPath { get; set; }
    }
}