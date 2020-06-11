using System.Data;
using System;
using System.Collections.Generic;
using LiteCommerce.DomainModels;
using Microsoft.Data.SqlClient;

namespace LiteCommerce.DataLayers
{
    public class EmployeeUserAccountDAL : IUserAccountDAL
    {
        private string connectionString;
        public EmployeeUserAccountDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public UserAccount Authenticate(string email, string password)
        {
            Employee employee = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT EmployeeID
                                    ,   LastName
                                    ,   FirstName
                                    ,   Title
                                    ,   Email
                                    ,   PhotoPath
                                    ,   Password
                                    ,   Roles
                                    FROM Employees WHERE Email = @email";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@email", email);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        employee = new Employee()
                        {
                            EmployeeID = Convert.ToInt32(dbReader["EmployeeID"]),
                            LastName = Convert.ToString(dbReader["LastName"]),
                            FirstName = Convert.ToString(dbReader["FirstName"]),
                            Title = Convert.ToString(dbReader["Title"]),
                            Email = Convert.ToString(dbReader["Email"]),
                            PhotoPath = Convert.ToString(dbReader["PhotoPath"]),
                            Password = Convert.ToString(dbReader["Password"]),
                            Roles = Convert.ToString(dbReader["Roles"]),
                        };
                    }
                }

                connection.Close();
            }
            if (employee != null)
            {
                if (employee.Password == password)
                {
                    return new UserAccount()
                    {
                        UserID = Convert.ToString(employee.EmployeeID),
                        Fullname = employee.LastName + " " + employee.FirstName,
                        Photo = employee.PhotoPath,
                        Title = employee.Title,
                    };
                }
            }

            return null;
        }

    }
}