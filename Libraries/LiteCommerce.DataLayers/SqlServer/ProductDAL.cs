using System.Data;
using System;
using System.Collections.Generic;
using LiteCommerce.DomainModels;
using Microsoft.Data.SqlClient;

namespace LiteCommerce.DataLayers.SqlServer
{
    public class ProductDAL : IProductDAL
    {
        private string connectionString;
        public ProductDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public List<Product> List(int page, int pageSize, string searchValue, string categoryID, string supplierID)
        {
            List<Product> listProduct = new List<Product>();
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            if (string.IsNullOrEmpty(categoryID))
            {
                categoryID = "";
            }
            if (string.IsNullOrEmpty(supplierID))
            {
                supplierID = "";
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM (
                                        SELECT ROW_NUMBER() over(order by ProductName) as RowNumber
                                        ,   Products.*
                                        ,   CategoryName
                                        ,   CompanyName
                                        FROM Products
                                            JOIN Categories 
                                                ON Products.CategoryID = Categories.CategoryID
                                            JOIN Suppliers 
                                                ON Products.SupplierID = Suppliers.SupplierID
                                        where
                                            ((@searchValue = N'') or (ProductName like @searchValue))
                                            AND ((@categoryID = N'') or (Products.CategoryID = @categoryID))
                                            AND ((@supplierID = N'') or (Products.SupplierID = @supplierID))
                                    ) as t
                                    where t.RowNumber between @pageSize * (@page -  1) + 1 and @page * @pageSize";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cmd.Parameters.AddWithValue("@searchValue", searchValue);
                cmd.Parameters.AddWithValue("@categoryID", categoryID);
                cmd.Parameters.AddWithValue("@supplierID", supplierID);

                using (SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dataReader.Read())
                    {
                        Category Category = new Category()
                        {
                            CategoryName = Convert.ToString(dataReader["CategoryName"])
                        };
                        Supplier Supplier = new Supplier()
                        {
                            CompanyName = Convert.ToString(dataReader["CompanyName"]),
                        };
                        listProduct.Add(new Product()
                        {
                            ProductID = Convert.ToInt32(dataReader["ProductID"]),
                            ProductName = Convert.ToString(dataReader["ProductName"]),
                            SupplierID = Convert.ToInt32(dataReader["SupplierID"]),
                            CategoryID = Convert.ToInt32(dataReader["CategoryID"]),
                            QuantityPerUnit = Convert.ToString(dataReader["QuantityPerUnit"]),
                            UnitPrice = Convert.ToInt32(dataReader["UnitPrice"]),
                            Descriptions = Convert.ToString(dataReader["Descriptions"]),
                            PhotoPath = Convert.ToString(dataReader["PhotoPath"]),
                            Category = Category,
                            Supplier = Supplier
                        });
                    }
                }

                conn.Close();
            }

            return listProduct;
        }

        public int Count(string searchValue, string categoryID, string supplierID)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            if (string.IsNullOrEmpty(categoryID))
            {
                categoryID = "";
            }
            if (string.IsNullOrEmpty(supplierID))
            {
                supplierID = "";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT COUNT(*)
                                    FROM Products
                                        JOIN Categories
                                            ON Categories.CategoryID = Products.CategoryID
                                        JOIN Suppliers
                                            ON Suppliers.SupplierID = Products.SupplierID
                                    WHERE ((@searchValue = N'') or (ProductName like @searchValue))
                                        AND ((@categoryID = N'') or (Products.categoryID = @categoryID))
                                        AND ((@supplierID = N'') or (Products.SupplierID = @supplierID))";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@searchValue", searchValue);
                cmd.Parameters.AddWithValue("@categoryID", categoryID);
                cmd.Parameters.AddWithValue("@supplierID", supplierID);

                count = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }
            return count;
        }

        public Product Get(int productID)
        {
            Product data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT 
                                        Products.*
                                    ,   CategoryName
                                    ,   CompanyName
                                    FROM Products
                                        JOIN Categories
                                            ON Categories.CategoryID = Products.CategoryID
                                        JOIN Suppliers
                                            ON Suppliers.SupplierID = Products.SupplierID
                                    WHERE ProductID = @productID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@productID", productID);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        Category Category = new Category()
                        {
                            CategoryName = Convert.ToString(dbReader["CategoryName"])
                        };
                        Supplier Supplier = new Supplier()
                        {
                            CompanyName = Convert.ToString(dbReader["CompanyName"])
                        };
                        data = new Product()
                        {
                            ProductID = Convert.ToInt32(dbReader["ProductID"]),
                            ProductName = Convert.ToString(dbReader["ProductName"]),
                            SupplierID = Convert.ToInt32(dbReader["SupplierID"]),
                            CategoryID = Convert.ToInt32(dbReader["CategoryID"]),
                            QuantityPerUnit = Convert.ToString(dbReader["QuantityPerUnit"]),
                            UnitPrice = Convert.ToInt32(dbReader["UnitPrice"]),
                            Descriptions = Convert.ToString(dbReader["Descriptions"]),
                            PhotoPath = Convert.ToString(dbReader["PhotoPath"]),
                            Category = Category,
                            Supplier = Supplier
                        };
                    }
                }

                connection.Close();
            }
            return data;
        }

        public int Add(Product product)
        {
            int productId = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO Products
                                    (
                                        ProductName,
                                        SupplierID,
                                        CategoryID,
                                        QuantityPerUnit,
                                        UnitPrice,
                                        Descriptions,
                                        PhotoPath
                                    )
                                    VALUES
                                    (
                                        @ProductName,
                                        @SupplierID,
                                        @CategoryID,
                                        @QuantityPerUnit,
                                        @UnitPrice,
                                        @Descriptions,
                                        @PhotoPath
                                    );
                                    SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                cmd.Parameters.AddWithValue("@SupplierID", product.SupplierID);
                cmd.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                cmd.Parameters.AddWithValue("@QuantityPerUnit", product.QuantityPerUnit);
                cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                cmd.Parameters.AddWithValue("@Descriptions", product.Descriptions);
                cmd.Parameters.AddWithValue("@PhotoPath", product.PhotoPath);

                productId = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }

            return productId;
        }

        public bool Update(Product product)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE Products
                                    SET ProductName = @ProductName
                                    ,   SupplierID = @SupplierID
                                    ,   CategoryID = @CategoryID
                                    ,   QuantityPerUnit = @QuantityPerUnit
                                    ,   UnitPrice = @UnitPrice
                                    ,   Descriptions = @Descriptions
                                    ,   PhotoPath = @PhotoPath
                                    WHERE ProductID = @ProductID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@ProductID", product.ProductID);
                cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                cmd.Parameters.AddWithValue("@SupplierID", product.SupplierID);
                cmd.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                cmd.Parameters.AddWithValue("@QuantityPerUnit", product.QuantityPerUnit);
                cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                cmd.Parameters.AddWithValue("@Descriptions", product.Descriptions);
                cmd.Parameters.AddWithValue("@PhotoPath", product.PhotoPath);

                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());

                connection.Close();
            }

            return rowsAffected > 0;
        }

        public int Delete(int[] productIDs)
        {
            int countDeleted = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"DELETE FROM Products
                                    WHERE(ProductID = @productID)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add("@productID", SqlDbType.Int);
                foreach (int productID in productIDs)
                {
                    cmd.Parameters["@productID"].Value = productID;
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        countDeleted += 1;
                }

                connection.Close();
            }
            return countDeleted;
        }
    }
}