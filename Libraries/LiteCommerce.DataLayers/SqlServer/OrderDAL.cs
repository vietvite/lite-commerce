using System.Data;
using System;
using System.Collections.Generic;
using LiteCommerce.DomainModels;
using Microsoft.Data.SqlClient;

namespace LiteCommerce.DataLayers.SqlServer
{
    public class OrderDAL : IOrderDAL
    {
        private string connectionString;
        public OrderDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Order> List(int page, int pageSize, string countryID, string categoryID, string employeeID, string shipperID)
        {
            List<Order> listOrder = new List<Order>();
            if (categoryID == "0")
                categoryID = "";
            if (employeeID == "0")
                employeeID = "";
            if (shipperID == "0")
                shipperID = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select * from (
                                        select DISTINCT Orders.OrderID,
                                            DENSE_RANK() over(order by Orders.OrderID DESC) as RowNumber, 
                                            Orders.OrderDate,
                                            Orders.RequiredDate,
                                            Orders.ShippedDate,
                                            Orders.Freight,
                                            Orders.ShipAddress,
                                            Orders.ShipCity,
                                            Orders.ShipCountry,
                                            Shippers.CompanyName as ShipperCompanyName,
                                            Customers.CompanyName as CustomerCompanyName,
                                            Employees.FirstName,
                                            Employees.LastName
                                        from Orders
                                            JOIN Shippers
                                                ON Orders.ShipperID = Shippers.ShipperID
                                            JOIN Customers
                                                ON Orders.CustomerID = Customers.CustomerID
                                            JOIN Employees
                                                ON Orders.EmployeeID = Employees.EmployeeID
                                            JOIN OrderDetails
                                                ON Orders.OrderID = OrderDetails.OrderID
                                            JOIN Products
                                                ON OrderDetails.ProductID = Products.ProductID
                                            JOIN Country
                                                ON Country.CountryID = Orders.ShipCountry
                                            JOIN Categories
                                                ON Products.CategoryID = Categories.CategoryID
                                        where ((@countryID = N'') or (Country.CountryID = @countryID))
                                            AND ((@categoryID = N'') or (Categories.CategoryID = @categoryID))
                                            AND ((@employeeID = N'') or (Employees.EmployeeID = @employeeID))
                                            AND ((@shipperID = N'') or (Shippers.ShipperID = @shipperID))
                                    ) as t
                                    where
                                        (@pageSize = -1)
                                        or (t.RowNumber between @pageSize * (@page -  1) + 1 and @page * @pageSize)
                                    order by t.RowNumber";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cmd.Parameters.AddWithValue("@countryID", countryID);
                cmd.Parameters.AddWithValue("@categoryID", categoryID);
                cmd.Parameters.AddWithValue("@employeeID", employeeID);
                cmd.Parameters.AddWithValue("@shipperID", shipperID);

                using (SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dataReader.Read())
                    {
                        listOrder.Add(new Order()
                        {
                            OrderID = Convert.ToInt32(dataReader["OrderID"]),
                            OrderDate = Convert.ToDateTime(dataReader["OrderDate"]),
                            RequiredDate = Convert.ToDateTime(dataReader["RequiredDate"]),
                            ShippedDate = dataReader["ShippedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dataReader["ShippedDate"]),
                            Freight = Convert.ToDecimal(dataReader["Freight"]),
                            ShipAddress = Convert.ToString(dataReader["ShipAddress"]),
                            ShipCity = Convert.ToString(dataReader["ShipCity"]),
                            ShipCountry = Convert.ToString(dataReader["ShipCountry"]),

                            Shipper = new Shipper()
                            {
                                CompanyName = Convert.ToString(dataReader["ShipperCompanyName"]),
                            },
                            Customer = new Customer()
                            {
                                CompanyName = Convert.ToString(dataReader["CustomerCompanyName"]),
                            },
                            Employee = new Employee()
                            {
                                FirstName = Convert.ToString(dataReader["FirstName"]),
                                LastName = Convert.ToString(dataReader["LastName"]),
                            },
                        });
                    }
                }

                conn.Close();
            }

            return listOrder;
        }

        public int Count(string countryID, string categoryID, string employeeID, string shipperID)
        {
            int count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select COUNT(DISTINCT Orders.OrderID)
                                        from Orders
                                            JOIN Shippers
                                                ON Orders.ShipperID = Shippers.ShipperID
                                            JOIN Customers
                                                ON Orders.CustomerID = Customers.CustomerID
                                            JOIN Employees
                                                ON Orders.EmployeeID = Employees.EmployeeID
                                            JOIN OrderDetails
                                                ON Orders.OrderID = OrderDetails.OrderID
                                            JOIN Products
                                                ON OrderDetails.ProductID = Products.ProductID
                                            JOIN Country
                                                ON Country.CountryID = Orders.ShipCountry
                                            JOIN Categories
                                                ON Products.CategoryID = Categories.CategoryID
                                        where ((@countryID = N'') or (Country.CountryID = @countryID))
                                            AND ((@categoryID = N'') or (Categories.CategoryID = @categoryID))
                                            AND ((@employeeID = N'') or (Employees.EmployeeID = @employeeID))
                                            AND ((@shipperID = N'') or (Shippers.ShipperID = @shipperID))";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@countryID", countryID);
                cmd.Parameters.AddWithValue("@categoryID", categoryID);
                cmd.Parameters.AddWithValue("@employeeID", employeeID);
                cmd.Parameters.AddWithValue("@shipperID", shipperID);

                count = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }
            return count;
        }

        public List<OrderDetails> Get(int orderID)
        {
            List<OrderDetails> listOrderDetails = listOrderDetails = new List<OrderDetails>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select Orders.*,
                                        Shippers.CompanyName as ShipperCompanyName,
                                        Customers.CompanyName as CustomerCompanyName,
                                        Employees.FirstName,
                                        Employees.LastName,
                                        Products.ProductID,
                                        Products.ProductName,
                                        OrderDetails.UnitPrice,
                                        Quantity,
                                        Discount
                                    from Orders
                                        JOIN Shippers
                                            ON Orders.ShipperID = Shippers.ShipperID
                                        JOIN Customers
                                            ON Orders.CustomerID = Customers.CustomerID
                                        JOIN Employees
                                            ON Orders.EmployeeID = Employees.EmployeeID
                                        JOIN OrderDetails
                                            ON Orders.OrderID = OrderDetails.OrderID
                                        JOIN Products
                                            ON OrderDetails.ProductID = Products.ProductID
                                    where ((@orderID = N'') or (Orders.OrderID = @orderID))";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@orderID", orderID);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dbReader.Read())
                    {

                        listOrderDetails.Add(new OrderDetails()
                        {
                            OrderID = Convert.ToInt32(dbReader["OrderID"]),
                            OrderDate = Convert.ToDateTime(dbReader["OrderDate"]),
                            RequiredDate = Convert.ToDateTime(dbReader["RequiredDate"]),
                            ShippedDate = dbReader["ShippedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dbReader["ShippedDate"]),
                            Freight = Convert.ToDecimal(dbReader["Freight"]),
                            ShipAddress = Convert.ToString(dbReader["ShipAddress"]),
                            ShipCity = Convert.ToString(dbReader["ShipCity"]),
                            ShipCountry = Convert.ToString(dbReader["ShipCountry"]),

                            Shipper = new Shipper()
                            {
                                CompanyName = Convert.ToString(dbReader["ShipperCompanyName"]),
                            },
                            Customer = new Customer()
                            {
                                CompanyName = Convert.ToString(dbReader["CustomerCompanyName"]),
                            },
                            Employee = new Employee()
                            {
                                FirstName = Convert.ToString(dbReader["FirstName"]),
                                LastName = Convert.ToString(dbReader["LastName"]),
                            },

                            UnitPrice = Convert.ToDecimal(dbReader["UnitPrice"]),
                            Quantity = Convert.ToInt32(dbReader["Quantity"]),
                            Discount = Convert.ToDecimal(dbReader["Discount"]),
                            Product = new Product()
                            {
                                ProductID = Convert.ToInt32(dbReader["ProductID"]),
                                ProductName = Convert.ToString(dbReader["ProductName"]),
                            }
                        });
                    }
                }

                connection.Close();
            }
            return listOrderDetails;
        }

        public int Add(OrderDetails order)
        {
            int orderId = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO Orders
                                    (
                                        OrderDate,
                                        RequiredDate,
                                        ShippedDate,
                                        Freight,
                                        ShipAddress,
                                        ShipCity,
                                        ShipCountry,
                                        ShipperID,
                                        CustomerID,
                                        EmployeeID
                                    )
                                    VALUES
                                    (
                                        @OrderDate,
                                        @RequiredDate,
                                        @ShippedDate,
                                        @Freight,
                                        @ShipAddress,
                                        @ShipCity,
                                        @ShipCountry,
                                        @ShipperID,
                                        @CustomerID,
                                        @EmployeeID
                                    );
                                    SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                cmd.Parameters.AddWithValue("@RequiredDate", order.RequiredDate);
                cmd.Parameters.AddWithValue("@ShippedDate", order.ShippedDate);
                cmd.Parameters.AddWithValue("@Freight", order.Freight);
                cmd.Parameters.AddWithValue("@ShipAddress", order.ShipAddress);
                cmd.Parameters.AddWithValue("@ShipCity", order.ShipCity);
                cmd.Parameters.AddWithValue("@ShipCountry", order.ShipCountry);
                cmd.Parameters.AddWithValue("@ShipperID", order.Shipper.ShipperID);
                cmd.Parameters.AddWithValue("@CustomerID", order.Customer.CustomerID);
                cmd.Parameters.AddWithValue("@EmployeeID", order.Employee.EmployeeID);

                // Using last created OrderID of Order table to add data to OrderDetails table
                orderId = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = @"INSERT INTO OrderDetails
                                    (
                                        OrderID,
                                        ProductID,
                                        UnitPrice,
                                        Quantity,
                                        Discount
                                    )
                                    VALUES
                                    (
                                        @OrderID,
                                        @ProductID,
                                        @UnitPrice,
                                        @Quantity,
                                        @Discount
                                    );";

                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@ProductID", order.Product.ProductID);
                cmd.Parameters.AddWithValue("@UnitPrice", order.UnitPrice);
                cmd.Parameters.AddWithValue("@Quantity", order.Quantity);
                cmd.Parameters.AddWithValue("@Discount", order.Discount);

                cmd.ExecuteScalar();

                connection.Close();
            }

            return orderId;
        }

        public bool Update(OrderDetails order)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO OrderDetails
                                    (
                                        OrderID,
                                        ProductID,
                                        UnitPrice,
                                        Quantity,
                                        Discount
                                    )
                                    VALUES
                                    (
                                        @OrderID,
                                        @ProductID,
                                        @UnitPrice,
                                        @Quantity,
                                        @Discount
                                    );";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@OrderID", order.OrderID);
                cmd.Parameters.AddWithValue("@ProductID", order.Product.ProductID);
                cmd.Parameters.AddWithValue("@UnitPrice", order.UnitPrice);
                cmd.Parameters.AddWithValue("@Quantity", order.Quantity);
                cmd.Parameters.AddWithValue("@Discount", order.Discount);

                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());

                connection.Close();
            }

            return rowsAffected > 0;
        }

        public int Delete(int[] categoryIDs)
        {
            int countDeleted = 0;
            // using (SqlConnection connection = new SqlConnection(connectionString))
            // {
            //     connection.Open();

            //     SqlCommand cmd = new SqlCommand();
            //     cmd.CommandText = @"DELETE FROM Orders
            //                         WHERE(CategoryID = @categoryId)
            //                             AND(CategoryID NOT IN(SELECT CategoryID FROM Products))";
            //     cmd.CommandType = CommandType.Text;
            //     cmd.Connection = connection;
            //     cmd.Parameters.Add("@categoryId", SqlDbType.Int);
            //     foreach (int categoryId in categoryIDs)
            //     {
            //         cmd.Parameters["@categoryId"].Value = categoryId;
            //         int rowsAffected = cmd.ExecuteNonQuery();
            //         if (rowsAffected > 0)
            //             countDeleted += 1;
            //     }

            //     connection.Close();
            // }
            return countDeleted;
        }
    }
}