using System.Data;
using System;
using System.Collections.Generic;
using LiteCommerce.DomainModels;
using Microsoft.Data.SqlClient;

namespace LiteCommerce.DataLayers.SqlServer
{
    public class ProductAttributeDAL : IProductAttributeDAL
    {
        private string connectionString;
        public ProductAttributeDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<ProductAttribute> List(int ProductID)
        {
            List<ProductAttribute> listAttributes = new List<ProductAttribute>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select ROW_NUMBER() over(order by DisplayOrder) as RowNumber, ProductAttributes.*
                                    from ProductAttributes
                                    where (@ProductID = N'') or (ProductID like @ProductID)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@ProductID", ProductID);

                using (SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dataReader.Read())
                    {
                        listAttributes.Add(new ProductAttribute()
                        {
                            AttributeID = Convert.ToInt32(dataReader["AttributeID"]),
                            ProductID = Convert.ToInt32(dataReader["ProductID"]),
                            AttributeName = Convert.ToString(dataReader["AttributeName"]),
                            AttributeValues = Convert.ToString(dataReader["AttributeValues"]),
                            DisplayOrder = Convert.ToInt32(dataReader["DisplayOrder"]),
                        });
                    }
                }

                conn.Close();
            }

            return listAttributes;
        }

        // public ProductAttribute Get(int categoryID)
        // {
        //   ProductAttribute data = null;
        //   using (SqlConnection connection = new SqlConnection(connectionString))
        //   {
        //     connection.Open();

        //     SqlCommand cmd = new SqlCommand();
        //     cmd.CommandText = @"SELECT * FROM Categories WHERE CategoryID = @categoryID";
        //     cmd.CommandType = CommandType.Text;
        //     cmd.Connection = connection;
        //     cmd.Parameters.AddWithValue("@categoryID", categoryID);

        //     using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
        //     {
        //       if (dbReader.Read())
        //       {
        //         data = new ProductAttribute()
        //         {
        //           CategoryID = Convert.ToInt32(dbReader["CategoryID"]),
        //           CategoryName = Convert.ToString(dbReader["CategoryName"]),
        //           Description = Convert.ToString(dbReader["Description"]),
        //         };
        //       }
        //     }

        //     connection.Close();
        //   }
        //   return data;
        // }

        public int Add(ProductAttribute productAttribute)
        {
            int returnedProductAttributeId = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO ProductAttributes
                                  (
                                      ProductID,
                                      AttributeName,
                                      AttributeValues,
                                      DisplayOrder
                                  )
                                  VALUES
                                  (
                                      @ProductID,
                                      @AttributeName,
                                      @AttributeValues,
                                      @DisplayOrder
                                  );
                                  SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@ProductID", productAttribute.ProductID);
                cmd.Parameters.AddWithValue("@AttributeName", productAttribute.AttributeName);
                cmd.Parameters.AddWithValue("@AttributeValues", productAttribute.AttributeValues);
                cmd.Parameters.AddWithValue("@DisplayOrder", productAttribute.DisplayOrder);

                returnedProductAttributeId = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }

            return returnedProductAttributeId;
        }

        public bool Update(List<ProductAttribute> listAttributes)
        {
            int countRowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE ProductAttributes
                                    SET ProductID = @ProductID
                                    ,   AttributeName = @AttributeName
                                    ,   AttributeValues = @AttributeValues
                                    ,   DisplayOrder = @DisplayOrder
                                    WHERE AttributeID = @AttributeID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;

                cmd.Parameters.Add("@AttributeID", SqlDbType.Int);
                cmd.Parameters.Add("@ProductID", SqlDbType.Int);
                cmd.Parameters.Add("@AttributeName", SqlDbType.NVarChar);
                cmd.Parameters.Add("@AttributeValues", SqlDbType.NVarChar);
                cmd.Parameters.Add("@DisplayOrder", SqlDbType.Int);

                foreach (ProductAttribute attribute in listAttributes)
                {
                    cmd.Parameters["@AttributeID"].Value = attribute.AttributeID;
                    cmd.Parameters["@ProductID"].Value = attribute.ProductID;
                    cmd.Parameters["@AttributeName"].Value = attribute.AttributeName;
                    cmd.Parameters["@AttributeValues"].Value = attribute.AttributeValues;
                    cmd.Parameters["@DisplayOrder"].Value = attribute.DisplayOrder;

                    int rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());
                    if (rowsAffected > 0)
                        countRowsAffected += 1;
                }

                connection.Close();
            }

            return countRowsAffected > 0;
        }

        public int Delete(string ProductID, string AttributeID)
        {
            int countDeleted = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"DELETE FROM ProductAttributes
                                    WHERE (AttributeID = @AttributeID)
                                        AND (ProductID = @ProductID)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@ProductID", ProductID);
                cmd.Parameters.AddWithValue("@AttributeID", AttributeID);

                countDeleted = cmd.ExecuteNonQuery();

                connection.Close();
            }
            return countDeleted;
        }
    }
}