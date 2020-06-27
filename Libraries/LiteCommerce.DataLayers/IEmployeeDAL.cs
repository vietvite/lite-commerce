using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.DataLayers
{
    public interface IEmployeeDAL
    {
        List<Employee> List(int page, int pageSize, string searchValue, string country);

        int Count(string searchValue, string country);

        Employee Get(int employeeID);

        int Add(Employee employee);

        bool Update(Employee employee);

        int Delete(int[] employeeIDs);

        bool ChangePassword(Employee employee);
    }
}