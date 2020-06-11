using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using  System.Text;

namespace LiteCommerce.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string text)
        {
        try
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            byte[] dataMd5 = md5.ComputeHash(Encoding.Default.GetBytes(text));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dataMd5.Length; i++)
            sb.AppendFormat("{0:x2}", dataMd5[i]);
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        }
    }
}