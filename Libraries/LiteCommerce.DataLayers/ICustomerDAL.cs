using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.DataLayers
{
    public interface ICustomerDAL
    {
        List<Customer> List(int page, int pageSize, string searchValue, string country);

        int Count(string searchValue, string country);

        Customer Get(string customerID);

        string Add(Customer customer);

        bool Update(Customer customer);

        int Delete(string[] customerIDs);
    }
}