using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.DataLayers
{
    public interface ICountryDAL
    {
        List<Country> List(int page, int pageSize, string searchValue);

        int Count(string searchValue);

        Country Get(string countryID);

        string Add(Country country);

        bool Update(Country country);

        int Delete(string[] countryIDs);
    }
}