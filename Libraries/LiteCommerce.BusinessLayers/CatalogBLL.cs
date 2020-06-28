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
        public static void Initialize(string connectionString)
        {
            SupplierDB = new DataLayers.SqlServer.SupplierDAL(connectionString);
            CustomerDB = new DataLayers.SqlServer.CustomerDAL(connectionString);
            ShipperDB = new DataLayers.SqlServer.ShipperDAL(connectionString);
            EmployeeDB = new DataLayers.SqlServer.EmployeeDAL(connectionString);
            CategoryDB = new DataLayers.SqlServer.CategoryDAL(connectionString);
            ProductDB = new DataLayers.SqlServer.ProductDAL(connectionString);
            CountryDB = new DataLayers.SqlServer.CountryDAL(connectionString);
            OrderDB = new DataLayers.SqlServer.OrderDAL(connectionString);
            ProductAttributeDB = new DataLayers.SqlServer.ProductAttributeDAL(connectionString);
        }
        private static ISupplierDAL SupplierDB { get; set; }
        private static ICustomerDAL CustomerDB { get; set; }
        private static IShipperDAL ShipperDB { get; set; }
        private static IEmployeeDAL EmployeeDB { get; set; }
        private static ICategoryDAL CategoryDB { get; set; }
        private static IProductDAL ProductDB { get; set; }
        private static ICountryDAL CountryDB { get; set; }
        private static IOrderDAL OrderDB { get; set; }
        private static IProductAttributeDAL ProductAttributeDB { get; set; }

        #endregion

        #region Supplier
        public static List<Supplier> ListOfSupplier(int page, int pageSize, string searchValue, out int rowCount)
        {
            if (page < 1)
                page = 1;
            rowCount = SupplierDB.Count(searchValue);
            return SupplierDB.List(page, pageSize, searchValue);
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

        #endregion

        #region Customer
        public static List<Customer> ListOfCustomer(int page, int pageSize, string searchValue, out int rowCount, string country)
        {
            if (page < 1)
                page = 1;
            rowCount = CustomerDB.Count(searchValue, country);
            return CustomerDB.List(page, pageSize, searchValue, country);
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
        #endregion

        #region Shipper
        public static List<Shipper> ListOfShipper(int page, int pageSize, string searchValue, out int rowCount)
        {
            if (page < 1)
                page = 1;
            rowCount = ShipperDB.Count(searchValue);
            return ShipperDB.List(page, pageSize, searchValue);
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
        #endregion

        #region Employee
        public static List<Employee> ListOfEmployee(int page, int pageSize, string searchValue, out int rowCount, string country)
        {
            if (page < 1)
                page = 1;
            rowCount = EmployeeDB.Count(searchValue, country);
            return EmployeeDB.List(page, pageSize, searchValue, country);
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
        public static bool ChangePasswordEmployee(Employee employee)
        {
            return EmployeeDB.ChangePassword(employee);
        }
        public static bool UpdateEmployeeProfile(Employee employee)
        {
            return EmployeeDB.UpdateProfile(employee);
        }
        #endregion

        #region Category
        public static List<Category> ListOfCategory(int page, int pageSize, string searchValue, out int rowCount)
        {
            if (page < 1)
                page = 1;
            rowCount = CategoryDB.Count(searchValue);
            return CategoryDB.List(page, pageSize, searchValue);
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
        #endregion

        #region Product
        public static List<Product> ListOfProduct(int page, int pageSize, string searchValue, out int rowCount, string category, string supplier)
        {
            if (page < 1)
                page = 1;
            rowCount = ProductDB.Count(searchValue, category, supplier);
            return ProductDB.List(page, pageSize, searchValue, category, supplier);
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

        #region Country
        public static List<Country> ListOfCountry(int page, int pageSize, string searchValue, out int rowCount)
        {
            if (page < 1)
                page = 1;
            rowCount = CountryDB.Count(searchValue);
            return CountryDB.List(page, pageSize, searchValue);
        }
        public static Country GetCountry(string countryID)
        {
            return CountryDB.Get(countryID);
        }
        public static string AddCountry(Country country)
        {
            return CountryDB.Add(country);
        }
        public static bool UpdateCountry(Country country)
        {
            return CountryDB.Update(country);
        }
        public static int DeleteCountries(string[] countryIDs)
        {
            return CountryDB.Delete(countryIDs);
        }
        #endregion

        #region Order
        public static List<Order> ListOfOrder(
            int page,
            int pageSize,
            out int rowCount,
            string country,
            string category,
            string employee,
            string shipper
        )
        {
            rowCount = OrderDB.Count(country, category, employee, shipper);
            return OrderDB.List(page, pageSize, country, category, employee, shipper);
        }
        public static List<OrderDetails> GetOrder(int orderID)
        {
            return OrderDB.Get(orderID);
        }
        public static int AddOrder(OrderDetails order)
        {
            return OrderDB.Add(order);
        }
        public static bool UpdateOrder(OrderDetails order)
        {
            return OrderDB.Update(order);
        }
        public static int DeleteOrders(int[] orderIDs)
        {
            return OrderDB.Delete(orderIDs);
        }
        #endregion

        #region ProductAttribute
        public static List<ProductAttribute> ListOfAttribute(int ProductID)
        {
            return ProductAttributeDB.List(ProductID);
        }
        // public static List<OrderDetails> GetOrder(int orderID)
        // {
        //   return ProductAttributeDB.Get(orderID);
        // }
        public static int AddProductAttribute(ProductAttribute productAttribute)
        {
            return ProductAttributeDB.Add(productAttribute);
        }
        public static bool UpdateProductAttribute(List<ProductAttribute> listAttribute)
        {
            return ProductAttributeDB.Update(listAttribute);
        }
        public static int DeleteProductAttributes(string productID, string attributeID)
        {
            return ProductAttributeDB.Delete(productID, attributeID);
        }
        #endregion


    }
}