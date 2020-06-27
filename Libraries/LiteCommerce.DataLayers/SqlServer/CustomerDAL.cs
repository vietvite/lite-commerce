using System.Data;
using System;
using System.Collections.Generic;
using LiteCommerce.DomainModels;
using Microsoft.Data.SqlClient;

namespace LiteCommerce.DataLayers.SqlServer
{
    public class CustomerDAL : ICustomerDAL
    {
        private string connectionString;
        public CustomerDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public List<Customer> List(int page, int pageSize, string searchValue, string country)
        {
            List<Customer> listCustomer = new List<Customer>();
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select * from (
                                        select ROW_NUMBER() over(order by CompanyName) as RowNumber, Customers.*
                                        from Customers
                                        where 
                                            ((@searchValue = N'') or (CompanyName like @searchValue))
                                            AND 
                                            ((@country = N'') or (Country = @country))
                                    ) as t
                                    where (@pageSize = -1)
                                        OR (t.RowNumber between @pageSize * (@page -  1) + 1 and @page * @pageSize)
                                        order by t.RowNumber";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cmd.Parameters.AddWithValue("@searchValue", searchValue);
                cmd.Parameters.AddWithValue("@country", country);

                using (SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dataReader.Read())
                    {
                        listCustomer.Add(new Customer()
                        {
                            CustomerID = Convert.ToString(dataReader["CustomerID"]),
                            CompanyName = Convert.ToString(dataReader["CompanyName"]),
                            ContactName = Convert.ToString(dataReader["ContactName"]),
                            ContactTitle = Convert.ToString(dataReader["ContactTitle"]),
                            Address = Convert.ToString(dataReader["Address"]),
                            City = Convert.ToString(dataReader["City"]),
                            Country = Convert.ToString(dataReader["Country"]),
                            Phone = Convert.ToString(dataReader["Phone"]),
                            Fax = Convert.ToString(dataReader["Fax"]),
                        });
                    }
                }

                conn.Close();
            }

            return listCustomer;
        }

        public int Count(string searchValue, string country)
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
                cmd.CommandText = @"select COUNT(*) from Customers where ((@searchValue = N'') or (CompanyName like @searchValue))
                                                                        AND (@country = N'') or (Country = @country)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@searchValue", searchValue);
                cmd.Parameters.AddWithValue("@country", country);

                count = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }
            return count;
        }

        public Customer Get(string customerID)
        {
            Customer data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM Customers WHERE CustomerID = @customerID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@customerID", customerID);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        data = new Customer()
                        {
                            CustomerID = Convert.ToString(dbReader["CustomerID"]),
                            CompanyName = Convert.ToString(dbReader["CompanyName"]),
                            ContactName = Convert.ToString(dbReader["ContactName"]),
                            ContactTitle = Convert.ToString(dbReader["ContactTitle"]),
                            Address = Convert.ToString(dbReader["Address"]),
                            City = Convert.ToString(dbReader["City"]),
                            Country = Convert.ToString(dbReader["Country"]),
                            Phone = Convert.ToString(dbReader["Phone"]),
                            Fax = Convert.ToString(dbReader["Fax"]),
                        };
                    }
                }

                connection.Close();
            }
            return data;
        }

        public string Add(Customer customer)
        {
            string CustomerID = "0";
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO Customers
                                    (
                                        CustomerID,
                                        CompanyName,
                                        ContactName,
                                        ContactTitle,
                                        Address,
                                        City,
                                        Country,
                                        Phone,
                                        Fax
                                    )
                                    VALUES
                                    (
                                        @CustomerID,
                                        @CompanyName,
                                        @ContactName,
                                        @ContactTitle,
                                        @Address,
                                        @City,
                                        @Country,
                                        @Phone,
                                        @Fax
                                    );
                                    SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                cmd.Parameters.AddWithValue("@CompanyName", customer.CompanyName);
                cmd.Parameters.AddWithValue("@ContactName", customer.ContactName);
                cmd.Parameters.AddWithValue("@ContactTitle", customer.ContactTitle);
                cmd.Parameters.AddWithValue("@Address", customer.Address);
                cmd.Parameters.AddWithValue("@City", customer.City);
                cmd.Parameters.AddWithValue("@Country", customer.Country);
                cmd.Parameters.AddWithValue("@Phone", customer.Phone);
                cmd.Parameters.AddWithValue("@Fax", customer.Fax);

                CustomerID = Convert.ToString(cmd.ExecuteScalar());

                connection.Close();
            }

            return CustomerID;
        }

        public bool Update(Customer customer)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE Customers
                                    SET CompanyName = @CompanyName 
                                        ,ContactName = @ContactName
                                        ,ContactTitle = @ContactTitle
                                        ,Address = @Address
                                        ,City = @City
                                        ,Country = @Country
                                        ,Phone = @Phone
                                        ,Fax = @Fax
                                    WHERE CustomerID = @CustomerID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                cmd.Parameters.AddWithValue("@CompanyName", customer.CompanyName);
                cmd.Parameters.AddWithValue("@ContactName", customer.ContactName);
                cmd.Parameters.AddWithValue("@ContactTitle", customer.ContactTitle);
                cmd.Parameters.AddWithValue("@Address", customer.Address);
                cmd.Parameters.AddWithValue("@City", customer.City);
                cmd.Parameters.AddWithValue("@Country", customer.Country);
                cmd.Parameters.AddWithValue("@Phone", customer.Phone);
                cmd.Parameters.AddWithValue("@Fax", customer.Fax);

                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());

                connection.Close();
            }

            return rowsAffected > 0;
        }

        public int Delete(string[] customerIDs)
        {
            int countDeleted = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"DELETE FROM Customers
                                    WHERE(CustomerID = @customerId)
                                        AND(CustomerID NOT IN(SELECT CustomerID FROM Orders))";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add("@customerId", SqlDbType.NChar);
                foreach (string customerID in customerIDs)
                {
                    cmd.Parameters["@customerId"].Value = customerID;
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