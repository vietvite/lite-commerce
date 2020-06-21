using System.Data;
using System;
using System.Collections.Generic;
using LiteCommerce.DomainModels;
using Microsoft.Data.SqlClient;

namespace LiteCommerce.DataLayers.SqlServer
{
    public class CountryDAL : ICountryDAL
    {
        private string connectionString;
        public CountryDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Country> List(int page, int pageSize, string searchValue)
        {
            List<Country> listCountry = new List<Country>();
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select * from (
	                                    select ROW_NUMBER() over(order by CountryName) as RowNumber, Country.*
	                                    from Country
	                                    where (@searchValue = N'') or (CountryName like @searchValue)
                                            or (@searchValue = N'') or (CountryID like @searchValue)
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
                        listCountry.Add(new Country()
                        {
                            CountryID = Convert.ToString(dataReader["CountryID"]),
                            CountryName = Convert.ToString(dataReader["CountryName"]),
                        });
                    }
                }

                conn.Close();
            }

            return listCountry;
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
                cmd.CommandText = @"select COUNT(*) from Country where ((@searchValue = N'') or (CountryName like @searchValue))
                                                                    or ((@searchValue = N'') or (CountryID like @searchValue))";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@searchValue", searchValue);

                count = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }
            return count;
        }

        public Country Get(string countryID)
        {
            Country data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM Country WHERE CountryID = @countryID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@countryID", countryID);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        data = new Country()
                        {
                            CountryID = Convert.ToString(dbReader["CountryID"]),
                            CountryName = Convert.ToString(dbReader["CountryName"]),
                        };
                    }
                }

                connection.Close();
            }
            return data;
        }

        public string Add(Country country)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO Country
                                    (
                                        CountryID,
                                        CountryName
                                    )
                                    VALUES
                                    (
                                        @countryID,
                                        @countryName
                                    );";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@countryID", country.CountryID);
                cmd.Parameters.AddWithValue("@countryName", country.CountryName);


                int n = cmd.ExecuteNonQuery();
                connection.Close();

                if (n > 0)
                {
                    return country.CountryID;
                }
                else
                {
                    return "";
                }
            }
        }

        public bool Update(Country country)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE Country
                                    SET CountryName = @CountryName
                                    WHERE CountryID = @CountryID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@CountryID", country.CountryID);
                cmd.Parameters.AddWithValue("@CountryName", country.CountryName);

                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());

                connection.Close();
            }

            return rowsAffected > 0;
        }

        public int Delete(string[] countryIDs)
        {
            int countDeleted = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"DELETE FROM Country
                                    WHERE(CountryID = @countryId)
                                        AND(CountryID NOT IN(SELECT ShipCountry FROM Orders))
                                        AND(CountryID NOT IN(SELECT Country FROM Customers))
                                        AND(CountryID NOT IN(SELECT Country FROM Employees))
                                        AND(CountryID NOT IN(SELECT Country FROM Suppliers))
                                    ";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add("@countryId", SqlDbType.NVarChar);
                foreach (string countryId in countryIDs)
                {
                    cmd.Parameters["@countryId"].Value = countryId;
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