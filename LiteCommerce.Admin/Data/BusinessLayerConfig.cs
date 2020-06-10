using Microsoft.Extensions.Configuration;
using LiteCommerce.BusinessLayers;

namespace LiteCommerce.Data
{
    public static class BusinessLayerConfig
    {
        public static void Init(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("ApplicationDbContextConnection");
            CatalogBLL.Initialize(connectionString);
        }
    }
}