using System.Collections.Generic;
using LiteCommerce.DomainModels;

namespace LiteCommerce.DataLayers
{
    public interface ISupplierDAL
    {
        /// <summary>
        /// Hiển thị danh sách suppliers, phân trang và có thể tìm kiếm
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        List<Supplier> List(int page, int pageSize, string searchValue);

        /// <summary>
        /// Đếm số lượng kết quả tìm kiếm được
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        int Count(string searchValue);

        /// <summary>
        /// Trả về một supplier
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        Supplier Get(int supplierID);

        /// <summary>
        /// Bổ sung một supplier. Hàm trả về id của sipplier được bổ sung.
        /// Nếu lỗi hàm trả về giá trị nhỏ hơn hoặc bằng 0
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        int Add(Supplier supplier);

        /// <summary>
        /// Cập nhập một supplier
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        bool Update(Supplier supplier);

        /// <summary>
        /// Xóa nhiều suppliers
        /// </summary>
        /// <param name="suppliers"></param>
        /// <returns></returns>
        int Delete(int[] supplierIDs);
    }
}