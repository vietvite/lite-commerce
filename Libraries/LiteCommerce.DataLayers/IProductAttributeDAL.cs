using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.DataLayers
{
    public interface IProductAttributeDAL
    {
        List<ProductAttribute> List(int ProductID);

        int Add(ProductAttribute productAttribute);

        bool Update(List<ProductAttribute> productAttribute);

        int Delete(string productID, string attributeID);
    }
}