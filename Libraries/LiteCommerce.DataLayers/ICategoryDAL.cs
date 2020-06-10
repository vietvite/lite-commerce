using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.DataLayers
{
    public interface ICategoryDAL
    {
        List<Category> List(int page, int pageSize, string searchValue);

        int Count(string searchValue);

        Category Get(int categoryID);

        int Add(Category category);

        bool Update(Category category);

        int Delete(int[] categoryIDs);
    }
}