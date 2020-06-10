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
        public List<Product> List(int page, int pageSize, string searchValue, string category, string supplier)
        {
            List<Product> listProduct = new List<Product>();
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            if (!string.IsNullOrEmpty(category))
            {
                category = "%" + category + "%";
            }
            if (!string.IsNullOrEmpty(supplier))
            {
                supplier = "%" + supplier + "%";
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select * from (
                                    select ROW_NUMBER() over(order by ProductName) as RowNumber, Products.*, CategoryName, [Description]
                                    from Products JOIN Categories ON Products.CategoryID = Categories.CategoryID
                                    where
                                    (@searchValue = N'') or (ProductName like @searchValue)
                                    AND (@category = N'') or (Products.CategoryID = @category)
                                    AND (@supplier = N'') or (SupplierID = @supplier)
                                ) as t
                                where t.RowNumber between @pageSize * (@page -  1) + 1 and @page * @pageSize";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cmd.Parameters.AddWithValue("@searchValue", searchValue);
                cmd.Parameters.AddWithValue("@category", category);
                cmd.Parameters.AddWithValue("@supplier", supplier);

                using (SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dataReader.Read())
                    {
                        Category Category = new Category()
                        {
                            CategoryID = Convert.ToInt32(dataReader["CategoryID"]),
                            CategoryName = Convert.ToString(dataReader["Description"]),
                            Description = Convert.ToString(dataReader["Description"])
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
                            Category = Category
                        });
                    }
                }

                conn.Close();
            }

            return listProduct;
        }

        public int Count(string searchValue)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select COUNT(*) from Products where @searchValue = N'' or ProductName like @searchValue";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@searchValue", searchValue);

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
                cmd.CommandText = @"SELECT * FROM Products WHERE ProductID = @productID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@productID", productID);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
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
                        };
                    }
                }

                connection.Close();
            }
            return data;
        }

        public int Add(Product category)
        {
            int categoryId = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO Products
                                    (
                                        ProductName,
                                        Descriptions
                                    )
                                    VALUES
                                    (
                                        @ProductName,
                                        @Descriptions
                                    );
                                    SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@ProductID", category.ProductID);
                cmd.Parameters.AddWithValue("@ProductName", category.ProductName);
                cmd.Parameters.AddWithValue("@Descriptions", category.Descriptions);

                categoryId = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }

            return categoryId;
        }

        public bool Update(Product category)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE Products
                                    SET ProductName = @ProductName
                                        ,Descriptions = @Descriptions
                                    WHERE ProductID = @ProductID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@ProductID", category.ProductID);
                cmd.Parameters.AddWithValue("@ProductName", category.ProductName);
                cmd.Parameters.AddWithValue("@Descriptions", category.Descriptions);

                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());

                connection.Close();
            }

            return rowsAffected > 0;
        }

        public int Delete(int[] categoryIDs)
        {
            int countDeleted = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"DELETE FROM Products
                                    WHERE(ProductID = @categoryId)
                                        AND(ProductID NOT IN(SELECT ProductID FROM Products))";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add("@categoryId", SqlDbType.Int);
                foreach (int categoryId in categoryIDs)
                {
                    cmd.Parameters["@categoryId"].Value = categoryId;
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