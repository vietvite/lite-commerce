using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.DataLayers
{
    public interface IOrderDAL
    {
        List<Order> List(int page, int pageSize, string country, string category, string employee, string shipper);

        int Count(string country, string category, string employee, string shipper);

        List<OrderDetails> Get(int orderID);

        int Add(OrderDetails order);

        bool Update(OrderDetails order);

        int Delete(int[] orderIDs);
    }
}