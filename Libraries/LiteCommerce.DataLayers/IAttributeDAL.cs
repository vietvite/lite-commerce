using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.DataLayers
{
    public interface IAttributeDAL
    {
        List<Attribute> List(string categoryID);

        int Count(string categoryID);

        Attribute Get(int attributeID);

        int Add(Attribute category);

        bool Update(Attribute category);

        int Delete(int[] categoryIDs);
    }
}