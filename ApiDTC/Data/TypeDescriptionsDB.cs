namespace ApiDTC.Data
{
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    public class TypeDescriptionsDb
    {   

        #region Attributes
        private readonly string _connectionString;
        #endregion

        #region Constructor
        public TypeDescriptionsDb(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods
        public SqlResult GetTypeDescriptionsData()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand descriptionsCommand = new SqlCommand("Select * From TypeDescriptions", sql);
                    
                    sql.Open();
                    if(sql.State != ConnectionState.Open)
                    {
                        return new SqlResult
                        {
                            Message = "SQL connection is closed",
                            Result = null
                        };
                    }
                    
                    var response = new List<SelectListItem>();

                    var reader = descriptionsCommand.ExecuteReader();
                    if(reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            response.Add(new SelectListItem
                            {
                                Value = reader["TypeDescriptionId"].ToString(),
                                Text = reader["Description"].ToString()

                            });
                        }
                        sql.Close();
                        return new SqlResult
                        {
                            Message = "Ok",
                            Result = response
                        };
                    }
                    else
                    {
                        return new SqlResult
                        {
                            Message = "Empty result",
                            Result = null
                        };
                    }
                }
                catch (SqlException ex)
                {
                    return new SqlResult
                    {
                        Message = $"Error: {ex.Message}",
                        Result = null
                    };
                }
            }
        }

        private TypeDescriptions MapToTypeDescriptions(SqlDataReader reader)
        {
            return new TypeDescriptions()
            {
                TypeDescriptionId = (int)reader["TypeDescriptionId"],
                Description = reader["Description"].ToString(),
            };
        }
        #endregion
    }
}
