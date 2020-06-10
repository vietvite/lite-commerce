using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.DataLayers
{
    public interface IShipperDAL
    {
        List<Shipper> List(int page, int pageSize, string searchValue);

        int Count(string searchValue);

        Shipper Get(int shipperID);

        int Add(Shipper shipper);

        bool Update(Shipper shipper);

        int Delete(int[] shipperIDs);
    }
}