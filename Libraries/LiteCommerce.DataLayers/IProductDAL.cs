using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.DataLayers
{
    public interface IProductDAL
    {
        List<Product> List(int page, int pageSize, string searchValue, string category, string supplier);

        int Count(string searchValue);

        Product Get(int productID);

        int Add(Product product);

        bool Update(Product product);

        int Delete(int[] productIDs);
    }
}