using LiteCommerce.DomainModels;
using LiteCommerce.DataLayers;

namespace LiteCommerce.BusinessLayers
{
    /// <summary>
    /// Provide methods for user account management
    /// </summary>
    public static class UserAccountBLL
    {
        private static string _connectionString;

        public static void Initialize(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static UserAccount Authenticate(string email, string password, UserAccountTypes userTypes)
        {
            IUserAccountDAL userAccountDB;
            switch (userTypes)
            {
                case UserAccountTypes.Employee:
                    userAccountDB = new EmployeeUserAccountDAL(_connectionString);
                    break;
                case UserAccountTypes.Customer:
                    userAccountDB = new CustomerUserAccountDAL(_connectionString);
                    break;
                default:
                    return null;
            }
            return userAccountDB.Authenticate(email, password);
        }

        public static UserAccount GetAccount(string email, UserAccountTypes userTypes)
        {
            IUserAccountDAL userAccountDB;
            switch (userTypes)
            {
                case UserAccountTypes.Employee:
                    userAccountDB = new EmployeeUserAccountDAL(_connectionString);
                    break;
                case UserAccountTypes.Customer:
                    userAccountDB = new CustomerUserAccountDAL(_connectionString);
                    break;
                default:
                    return null;
            }
            return userAccountDB.GetAccount(email);
        }
    }
}