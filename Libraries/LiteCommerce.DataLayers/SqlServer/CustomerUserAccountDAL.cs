using LiteCommerce.DomainModels;

namespace LiteCommerce.DataLayers
{
    public class CustomerUserAccountDAL : IUserAccountDAL
    {
        private string connectionString;
        public CustomerUserAccountDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public UserAccount Authenticate(string email, string password)
        {
            // TODO: Validate login rely on `Customers` tables
            return new UserAccount()
            {
                UserID = "asd",
                Fullname = "Van Viet",
                Photo = "asd.jpg"
            };
        }

    }
}