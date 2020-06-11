using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.DataLayers
{
    public interface IUserAccountDAL
    {
        /// <summary>
        /// Kiểm tra email & password hợp lệ.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>UserAccount|null</returns>
        UserAccount Authenticate(string email, string password);
    }
}