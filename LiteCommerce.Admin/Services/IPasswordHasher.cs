namespace LiteCommerce.Services
{
    public interface IPasswordHasher
    {
        string Hash(string password);
    }
}