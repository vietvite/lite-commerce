using System.Collections.Generic;
using LiteCommerce.DataLayers;
using LiteCommerce.DomainModels;

namespace LiteCommerce.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng xử lý nghiệp vụ xử lý dữ liệu chung như:
    /// - Nhà cung cấp
    /// - Khách hàng
    /// - Mặt hàng
    /// - ...
    /// </summary>
    public static class CatalogBLL
    {
        #region Khai báo các thuộc tính giao tiếp với DAL

        /// <summary>
        /// Hàm này phải được gọi để khởi tạo chức năng tác nghiệp
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Initialize(string connectionString = "Server=localhost;Database=LiteCommerce;User Id=sa;Password=asdASD123;MultipleActiveResultSets=true")
        {
            SupplierDB = new DataLayers.SqlServer.SupplierDAL(connectionString);
            CustomerDB = new DataLayers.SqlServer.CustomerDAL(connectionString);
            ShipperDB = new DataLayers.SqlServer.ShipperDAL(connectionString);
            EmployeeDB = new DataLayers.SqlServer.EmployeeDAL(connectionString);
            CategoryDB = new DataLayers.SqlServer.CategoryDAL(connectionString);
            ProductDB = new DataLayers.SqlServer.ProductDAL(connectionString);
        }
        private static ISupplierDAL SupplierDB { get; set; }
        private static ICustomerDAL CustomerDB { get; set; }
        private static IShipperDAL ShipperDB { get; set; }
        private static IEmployeeDAL EmployeeDB { get; set; }
        private static ICategoryDAL CategoryDB { get; set; }
        private static IProductDAL ProductDB { get; set; }

        #endregion

        #region Khai báo các chức năng xử lý nghiệp vụ
        public static List<Supplier> ListOfSupplier(int page, int pageSize, string searchValue, out int rowCount)
        {
            if (page < 1)
                page = 1;
            // if (pageSize < 0)
            //     pageSize = 1;
            rowCount = SupplierDB.Count(searchValue);
            return SupplierDB.List(page, pageSize, searchValue);
        }
        public static List<Customer> ListOfCustomer(int page, int pageSize, string searchValue, out int rowCount)
        {
            if (page < 1)
                page = 1;
            if (pageSize < 0)
                pageSize = 1;
            rowCount = CustomerDB.Count(searchValue);
            return CustomerDB.List(page, pageSize, searchValue);
        }
        public static List<Shipper> ListOfShipper(int page, int pageSize, string searchValue, out int rowCount)
        {
            if (page < 1)
                page = 1;
            if (pageSize < 0)
                pageSize = 1;
            rowCount = ShipperDB.Count(searchValue);
            return ShipperDB.List(page, pageSize, searchValue);
        }
        public static List<Employee> ListOfEmployee(int page, int pageSize, string searchValue, out int rowCount)
        {
            if (page < 1)
                page = 1;
            if (pageSize < 0)
                pageSize = 1;
            rowCount = EmployeeDB.Count(searchValue);
            return EmployeeDB.List(page, pageSize, searchValue);
        }
        public static List<Category> ListOfCategory(int page, int pageSize, string searchValue, out int rowCount)
        {
            if (page < 1)
                page = 1;
            // if (pageSize < 0)
            //     pageSize = 1;
            rowCount = CategoryDB.Count(searchValue);
            return CategoryDB.List(page, pageSize, searchValue);
        }

        public static List<Product> ListOfProduct(int page, int pageSize, string searchValue, out int rowCount, string category, string supplier)
        {
            if (page < 1)
                page = 1;
            // if (pageSize < 0)
            //     pageSize = 1;
            rowCount = CategoryDB.Count(searchValue);
            return ProductDB.List(page, pageSize, searchValue, category, supplier);
        }

        public static Supplier GetSupplier(int supplierID)
        {
            return SupplierDB.Get(supplierID);
        }
        public static int AddSupplier(Supplier supplier)
        {
            return SupplierDB.Add(supplier);
        }
        public static bool UpdateSupplier(Supplier supplier)
        {
            return SupplierDB.Update(supplier);
        }
        public static int DeleteSuppliers(int[] supplierID)
        {
            return SupplierDB.Delete(supplierID);
        }


        public static Category GetCategory(int categoryID)
        {
            return CategoryDB.Get(categoryID);
        }
        public static int AddCategory(Category category)
        {
            return CategoryDB.Add(category);
        }
        public static bool UpdateCategory(Category category)
        {
            return CategoryDB.Update(category);
        }
        public static int DeleteCategories(int[] categoryID)
        {
            return CategoryDB.Delete(categoryID);
        }


        public static Customer GetCustomer(string customerID)
        {
            return CustomerDB.Get(customerID);
        }
        public static string AddCustomer(Customer customer)
        {
            return CustomerDB.Add(customer);
        }
        public static bool UpdateCustomer(Customer customer)
        {
            return CustomerDB.Update(customer);
        }
        public static int DeleteCustomers(string[] customerID)
        {
            return CustomerDB.Delete(customerID);
        }

        public static Shipper GetShipper(int shipperID)
        {
            return ShipperDB.Get(shipperID);
        }
        public static int AddShipper(Shipper shipper)
        {
            return ShipperDB.Add(shipper);
        }
        public static bool UpdateShipper(Shipper shipper)
        {
            return ShipperDB.Update(shipper);
        }
        public static int DeleteShippers(int[] shipperID)
        {
            return ShipperDB.Delete(shipperID);
        }

        public static Employee GetEmployee(int employeeID)
        {
            return EmployeeDB.Get(employeeID);
        }
        public static int AddEmployee(Employee employee)
        {
            return EmployeeDB.Add(employee);
        }
        public static bool UpdateEmployee(Employee employee)
        {
            return EmployeeDB.Update(employee);
        }
        public static int DeleteEmployees(int[] employeeID)
        {
            return EmployeeDB.Delete(employeeID);
        }

        public static Product GetProduct(int productID)
        {
            return ProductDB.Get(productID);
        }
        public static int AddProduct(Product product)
        {
            return ProductDB.Add(product);
        }
        public static bool UpdateProduct(Product product)
        {
            return ProductDB.Update(product);
        }
        public static int DeleteProducts(int[] productIDs)
        {
            return ProductDB.Delete(productIDs);
        }


        #endregion
    }


}