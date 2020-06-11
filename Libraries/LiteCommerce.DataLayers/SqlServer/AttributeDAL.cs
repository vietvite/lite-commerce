using System.Data;
using System;
using System.Collections.Generic;
using LiteCommerce.DomainModels;
using Microsoft.Data.SqlClient;

namespace LiteCommerce.DataLayers.SqlServer
{
    public class AttributeDAL : IAttributeDAL
    {
        private string connectionString;
        public AttributeDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<LiteCommerce.DomainModels.Attribute> List(string categoryID)
        {
            List<LiteCommerce.DomainModels.Attribute> listAttribute = new List<LiteCommerce.DomainModels.Attribute>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select *
	                                    from Attribute
	                                    where (@categoryID = N'') or (CategoryID = @categoryID)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@categoryID", categoryID);

                using (SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dataReader.Read())
                    {
                        listAttribute.Add(new LiteCommerce.DomainModels.Attribute()
                        {
                            CategoryID = Convert.ToInt32(dataReader["CategoryID"]),
                            AttributeID = Convert.ToInt32(dataReader["AttributeID"]),
                            AttributeName = Convert.ToString(dataReader["AttributeName"]),
                        });
                    }
                }

                conn.Close();
            }

            return listAttribute;
        }

        public int Count(string categoryID)
        {
            int count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select COUNT(*) from Attribute where (@categoryID = N'') or (CategoryID = @categoryID)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@categoryID", categoryID);

                count = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }
            return count;
        }

        public LiteCommerce.DomainModels.Attribute Get(int attributeID)
        {
            LiteCommerce.DomainModels.Attribute data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM Attribute WHERE AttributeID = @attributeID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@attributeID", attributeID);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        data = new LiteCommerce.DomainModels.Attribute()
                        {
                            AttributeID = Convert.ToInt32(dbReader["AttributeID"]),
                            CategoryID = Convert.ToInt32(dbReader["CategoryID"]),
                            AttributeName = Convert.ToString(dbReader["AttributeName"]),
                        };
                    }
                }

                connection.Close();
            }
            return data;
        }

        public int Add(LiteCommerce.DomainModels.Attribute attribute)
        {
            int categoryId = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO Attribute
                                    (
                                        CategoryID,
                                        AttributeName
                                    )
                                    VALUES
                                    (
                                        @CategoryID,
                                        @AttributeName
                                    );
                                    SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@CategoryID", attribute.CategoryID);
                cmd.Parameters.AddWithValue("@AttributeName", attribute.AttributeName);

                categoryId = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }

            return categoryId;
        }

        public bool Update(LiteCommerce.DomainModels.Attribute category)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE Attribute
                                    SET CategoryID = @CategoryID
                                    ,   AttributeName = @AttributeName
                                    WHERE AttributeID = @attributeID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@attributeID", category.AttributeID);
                cmd.Parameters.AddWithValue("@CategoryID", category.CategoryID);
                cmd.Parameters.AddWithValue("@AttributeName", category.AttributeName);

                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());

                connection.Close();
            }

            return rowsAffected > 0;
        }

        public int Delete(int[] attributeIDs)
        {
            int countDeleted = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"DELETE FROM Attribute
                                    WHERE(AttributeID = @attributeID)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add("@attributeID", SqlDbType.Int);
                foreach (int attributeID in attributeIDs)
                {
                    cmd.Parameters["@attributeID"].Value = attributeID;
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