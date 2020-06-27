using System.Data;
using System;
using System.Collections.Generic;
using LiteCommerce.DomainModels;
using Microsoft.Data.SqlClient;

namespace LiteCommerce.DataLayers.SqlServer
{
    public class EmployeeDAL : IEmployeeDAL
    {
        private string connectionString;
        public EmployeeDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public List<Employee> List(int page, int pageSize, string searchValue, string country)
        {
            List<Employee> listEmployee = new List<Employee>();
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            if (string.IsNullOrEmpty(country))
                country = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select * from (
	                                    select ROW_NUMBER() over(order by FirstName) as RowNumber, Employees.*
	                                    from Employees
	                                    where ((@searchValue = N'') or (FirstName like @searchValue) or (LastName like @searchValue) or (Country like @searchValue))
                                            AND ((@country = N'') or (Country = @country))
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
                        listEmployee.Add(new Employee()
                        {
                            EmployeeID = Convert.ToInt32(dataReader["EmployeeID"]),
                            LastName = Convert.ToString(dataReader["LastName"]),
                            FirstName = Convert.ToString(dataReader["FirstName"]),
                            Title = Convert.ToString(dataReader["Title"]),
                            BirthDate = Convert.ToDateTime(dataReader["BirthDate"]),
                            HireDate = Convert.ToDateTime(dataReader["HireDate"]),
                            Email = Convert.ToString(dataReader["Email"]),
                            Address = Convert.ToString(dataReader["Address"]),
                            City = Convert.ToString(dataReader["City"]),
                            Country = Convert.ToString(dataReader["Country"]),
                            HomePhone = Convert.ToString(dataReader["HomePhone"]),
                            Notes = Convert.ToString(dataReader["Notes"]),
                            PhotoPath = Convert.ToString(dataReader["PhotoPath"]),
                        });
                    }
                }

                conn.Close();
            }

            return listEmployee;
        }

        public int Count(string searchValue, string country)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            if (string.IsNullOrEmpty(country))
                country = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select COUNT(*) from Employees where ((@searchValue = N'') or (LastName like @searchValue) or (FirstName like @searchValue))
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

        public Employee Get(int employeeID)
        {
            Employee data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM Employees WHERE EmployeeID = @employeeID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@employeeID", employeeID);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        data = new Employee()
                        {
                            EmployeeID = Convert.ToInt32(dbReader["EmployeeID"]),
                            LastName = Convert.ToString(dbReader["LastName"]),
                            FirstName = Convert.ToString(dbReader["FirstName"]),
                            Title = Convert.ToString(dbReader["Title"]),
                            BirthDate = Convert.ToDateTime(dbReader["BirthDate"]),
                            HireDate = Convert.ToDateTime(dbReader["HireDate"]),
                            Email = Convert.ToString(dbReader["Email"]),
                            Address = Convert.ToString(dbReader["Address"]),
                            City = Convert.ToString(dbReader["City"]),
                            Country = Convert.ToString(dbReader["Country"]),
                            HomePhone = Convert.ToString(dbReader["HomePhone"]),
                            Notes = Convert.ToString(dbReader["Notes"]),
                            PhotoPath = Convert.ToString(dbReader["PhotoPath"]),
                            Password = Convert.ToString(dbReader["Password"]),
                        };
                    }
                }

                connection.Close();
            }
            return data;
        }

        public int Add(Employee employee)
        {
            int employeeID = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO Employees
                                    (
                                        LastName,
                                        FirstName,
                                        Title,
                                        BirthDate,
                                        HireDate,
                                        Email,
                                        Address,
                                        City,
                                        Country,
                                        HomePhone,
                                        Notes,
                                        Roles,
                                        Password,
                                        PhotoPath
                                    )
                                    VALUES
                                    (
                                        @LastName,
                                        @FirstName,
                                        @Title,
                                        @BirthDate,
                                        @HireDate,
                                        @Email,
                                        @Address,
                                        @City,
                                        @Country,
                                        @HomePhone,
                                        @Notes,
                                        @Roles,
                                        @Password,
                                        @PhotoPath
                                    );
                                    SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@LastName", employee.LastName);
                cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
                cmd.Parameters.AddWithValue("@Title", employee.Title);
                cmd.Parameters.AddWithValue("@BirthDate", employee.BirthDate);
                cmd.Parameters.AddWithValue("@HireDate", employee.HireDate);
                cmd.Parameters.AddWithValue("@Email", employee.Email);
                cmd.Parameters.AddWithValue("@Address", employee.Address);
                cmd.Parameters.AddWithValue("@City", employee.City);
                cmd.Parameters.AddWithValue("@Country", employee.Country);
                cmd.Parameters.AddWithValue("@HomePhone", employee.HomePhone);
                cmd.Parameters.AddWithValue("@Notes", employee.Notes);
                cmd.Parameters.AddWithValue("@Roles", employee.Roles);
                cmd.Parameters.AddWithValue("@Password", employee.Password);
                cmd.Parameters.AddWithValue("@PhotoPath", employee.PhotoPath);

                employeeID = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }

            return employeeID;
        }

        public bool Update(Employee employee)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE Employees
                                    SET LastName = @LastName
                                    ,   FirstName = @FirstName
                                    ,   Title = @Title
                                    ,   BirthDate = @BirthDate
                                    ,   HireDate = @HireDate
                                    ,   Email = @Email
                                    ,   Address = @Address
                                    ,   City = @City
                                    ,   Country = @Country
                                    ,   HomePhone = @HomePhone
                                    ,   Notes = @Notes
                                    ,   PhotoPath = @PhotoPath
                                    WHERE EmployeeID = @EmployeeID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                cmd.Parameters.AddWithValue("@LastName", employee.LastName);
                cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
                cmd.Parameters.AddWithValue("@Title", employee.Title);
                cmd.Parameters.AddWithValue("@BirthDate", employee.BirthDate);
                cmd.Parameters.AddWithValue("@HireDate", employee.HireDate);
                cmd.Parameters.AddWithValue("@Email", employee.Email);
                cmd.Parameters.AddWithValue("@Address", employee.Address);
                cmd.Parameters.AddWithValue("@City", employee.City);
                cmd.Parameters.AddWithValue("@Country", employee.Country);
                cmd.Parameters.AddWithValue("@HomePhone", employee.HomePhone);
                cmd.Parameters.AddWithValue("@Notes", employee.Notes);
                cmd.Parameters.AddWithValue("@PhotoPath", employee.PhotoPath);

                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());

                connection.Close();
            }

            return rowsAffected > 0;
        }

        public int Delete(int[] employeeIDs)
        {
            int countDeleted = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"DELETE FROM Employees
                                    WHERE(EmployeeID = @employeeID)
                                        AND(EmployeeID NOT IN(SELECT EmployeeID FROM Orders))";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add("@employeeID", SqlDbType.Int);
                foreach (int employeeID in employeeIDs)
                {
                    cmd.Parameters["@employeeID"].Value = employeeID;
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        countDeleted += 1;
                }

                connection.Close();
            }
            return countDeleted;
        }

        public bool ChangePassword(Employee employee)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE Employees
                                    SET Password = @Password
                                    WHERE EmployeeID = @EmployeeID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                cmd.Parameters.AddWithValue("@Password", employee.Password);

                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());

                connection.Close();
            }

            return rowsAffected > 0;
        }
    }
}