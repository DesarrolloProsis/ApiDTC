namespace ApiDTC.Data
{
    using ApiDTC.Models;
    using ApiDTC.Services;
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

        private ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public TypeDescriptionsDb(IConfiguration configuration, ApiLogger apiLogger)
        {
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods

        //TODO Test TypeDescriptions
        public OperationResult GetTypeDescriptionsData()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand descriptionsCommand = new SqlCommand("Select * From TypeDescriptions", sql);
                    
                    sql.Open();
                    if(sql.State != ConnectionState.Open)
                    {
                        return new OperationResult
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
                        return new OperationResult
                        {
                            Message = "Ok",
                            Result = response
                        };
                    }
                    else
                    {
                        return new OperationResult
                        {
                            Message = "Empty result",
                            Result = null
                        };
                    }
                }
                catch (SqlException ex)
                {
                    _apiLogger.WriteLog(ex, "GetTypeDescriptionsData");
                    return new OperationResult
                    {
                        Message = $"Error: {ex.Message}",
                        Result = null
                    };
                }
            }
        }
        #endregion

        #region Mappers
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
