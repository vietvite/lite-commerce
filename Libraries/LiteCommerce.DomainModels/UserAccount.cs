namespace LiteCommerce.DomainModels
{
    /// <summary>
    /// Lưu thông tin liên quan đến tài khoản đăng nhập hệ thống
    /// </summary>
    public class UserAccount
    {
        public string UserID { get; set; }
        public string Fullname { get; set; }
        public string Photo { get; set; }
        public string Title { get; set; }
    }
}