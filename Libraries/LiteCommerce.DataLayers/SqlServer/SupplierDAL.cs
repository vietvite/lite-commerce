using System;
using System.Collections.Generic;
using LiteCommerce.DomainModels;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace LiteCommerce.DataLayers.SqlServer
{
    public class SupplierDAL : ISupplierDAL
    {
        private string connectionString;
        public SupplierDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// Hiển thị danh sách suppliers, phân trang và có thể tìm kiếm
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public List<Supplier> List(int page, int pageSize, string searchValue)
        {
            List<Supplier> listSupplier = new List<Supplier>();
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select * from (
	                                    select ROW_NUMBER() over(order by CompanyName) as RowNumber, Suppliers.*
	                                    from Suppliers
	                                    where (@searchValue = N'') or (CompanyName like @searchValue)
                                    ) as t
                                    where
                                        (@pageSize = -1)
                                        or (t.RowNumber between @pageSize * (@page -  1) + 1 and @page * @pageSize)
                                        order by t.RowNumber";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cmd.Parameters.AddWithValue("@searchValue", searchValue);

                using (SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dataReader.Read())
                    {
                        listSupplier.Add(new Supplier()
                        {
                            SupplierId = Convert.ToInt32(dataReader["SupplierID"]),
                            CompanyName = Convert.ToString(dataReader["CompanyName"]),
                            ContactName = Convert.ToString(dataReader["ContactName"]),
                            ContactTitle = Convert.ToString(dataReader["ContactTitle"]),
                            Address = Convert.ToString(dataReader["Address"]),
                            City = Convert.ToString(dataReader["City"]),
                            Country = Convert.ToString(dataReader["Country"]),
                            Phone = Convert.ToString(dataReader["Phone"]),
                            Fax = Convert.ToString(dataReader["Fax"]),
                            HomePage = Convert.ToString(dataReader["HomePage"])
                        });
                    }
                }

                conn.Close();
            }

            return listSupplier;
        }

        /// <summary>
        /// Đếm số lượng kết quả tìm kiếm được
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
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
                cmd.CommandText = "select COUNT(*) from Suppliers where @searchValue = N'' or CompanyName like @searchValue";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@searchValue", searchValue);

                count = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }
            return count;
        }

        /// <summary>
        /// Trả về một supplier
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        public Supplier Get(int supplierID)
        {
            Supplier data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM Suppliers WHERE SupplierID = @supplierID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@supplierID", supplierID);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        data = new Supplier()
                        {
                            SupplierId = Convert.ToInt32(dbReader["SupplierID"]),
                            CompanyName = Convert.ToString(dbReader["CompanyName"]),
                            ContactName = Convert.ToString(dbReader["ContactName"]),
                            ContactTitle = Convert.ToString(dbReader["ContactTitle"]),
                            Address = Convert.ToString(dbReader["Address"]),
                            City = Convert.ToString(dbReader["City"]),
                            Country = Convert.ToString(dbReader["Country"]),
                            Phone = Convert.ToString(dbReader["Phone"]),
                            Fax = Convert.ToString(dbReader["Fax"]),
                            HomePage = Convert.ToString(dbReader["HomePage"])
                        };
                    }
                }

                connection.Close();
            }
            return data;
        }

        /// <summary>
        /// Bổ sung một supplier. Hàm trả về id của sipplier được bổ sung.
        /// Nếu lỗi hàm trả về giá trị nhỏ hơn hoặc bằng 0
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public int Add(Supplier supplier)
        {
            int supplierId = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO Suppliers
                                    (
                                        CompanyName,
                                        ContactName,
                                        ContactTitle,
                                        Address,
                                        City,
                                        Country,
                                        Phone,
                                        Fax,
                                        HomePage
                                    )
                                    VALUES
                                    (
                                        @CompanyName,
                                        @ContactName,
                                        @ContactTitle,
                                        @Address,
                                        @City,
                                        @Country,
                                        @Phone,
                                        @Fax,
                                        @HomePage
                                    );
                                    SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@CompanyName", supplier.CompanyName);
                cmd.Parameters.AddWithValue("@ContactName", supplier.ContactName);
                cmd.Parameters.AddWithValue("@ContactTitle", supplier.ContactTitle);
                cmd.Parameters.AddWithValue("@Address", supplier.Address);
                cmd.Parameters.AddWithValue("@City", supplier.City);
                cmd.Parameters.AddWithValue("@Country", supplier.Country);
                cmd.Parameters.AddWithValue("@Phone", supplier.Phone);
                cmd.Parameters.AddWithValue("@Fax", supplier.Fax);
                cmd.Parameters.AddWithValue("@HomePage", supplier.HomePage);

                supplierId = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }

            return supplierId;
        }

        /// <summary>
        /// Cập nhập một supplier
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public bool Update(Supplier supplier)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE Suppliers
                                    SET CompanyName = @CompanyName 
                                        ,ContactName = @ContactName
                                        ,ContactTitle = @ContactTitle
                                        ,Address = @Address
                                        ,City = @City
                                        ,Country = @Country
                                        ,Phone = @Phone
                                        ,Fax = @Fax
                                        ,HomePage = @HomePage
                                    WHERE SupplierID = @SupplierID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@SupplierID", supplier.SupplierId);
                cmd.Parameters.AddWithValue("@CompanyName", supplier.CompanyName);
                cmd.Parameters.AddWithValue("@ContactName", supplier.ContactName);
                cmd.Parameters.AddWithValue("@ContactTitle", supplier.ContactTitle);
                cmd.Parameters.AddWithValue("@Address", supplier.Address);
                cmd.Parameters.AddWithValue("@City", supplier.City);
                cmd.Parameters.AddWithValue("@Country", supplier.Country);
                cmd.Parameters.AddWithValue("@Phone", supplier.Phone);
                cmd.Parameters.AddWithValue("@Fax", supplier.Fax);
                cmd.Parameters.AddWithValue("@HomePage", supplier.HomePage);

                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());

                connection.Close();
            }

            return rowsAffected > 0;
        }

        /// <summary>
        /// Xóa nhiều suppliers
        /// </summary>
        /// /// <param name="suppliers"></param>
        /// <returns></returns>
        public int Delete(int[] supplierIDs)
        {
            int countDeleted = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"DELETE FROM Suppliers
                                    WHERE(SupplierID = @supplierId)
                                        AND(SupplierID NOT IN(SELECT SupplierID FROM Products))";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add("@supplierId", SqlDbType.Int);
                foreach (int supplierId in supplierIDs)
                {
                    cmd.Parameters["@supplierId"].Value = supplierId;
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