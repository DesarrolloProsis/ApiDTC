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

        private readonly SqlResult _sqlResult;

        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public TypeDescriptionsDb(IConfiguration configuration, SqlResult sqlResult, ApiLogger apiLogger)
        {
            _apiLogger = apiLogger;
            _sqlResult = sqlResult;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods

        //TODO Test TypeDescriptions
        public Response GetTypeDescriptionsData(string clavePlaza)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    //SqlCommand descriptionsCommand = new SqlCommand("Select * From TypeDescriptions", sql);
                    SqlCommand descriptionsCommand = new SqlCommand("SELECT  ID, Description FROM TypesFaultDescriptions WHERE Catalog <> 0", sql); 
                    return _sqlResult.GetList<TypeDescriptions>(clavePlaza, descriptionsCommand, sql, "GetTypeDescriptionsData");
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "TypeDescriptionsDb: GetTypeDescriptionsData", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        public Response GetTypeDescriptionsId(string idType)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    SqlCommand descriptionsCommand = new SqlCommand($"SELECT  ID, Type FROM TypesFaultDescriptions WHERE SubCatalog = '{idType}'", sql);
                    return _sqlResult.GetList<TypeDTCDescription>(idType, descriptionsCommand, sql, "GetTypeDescriptionsId");
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(idType, ex, "TypeDescriptionsId: GetTypeDescriptionsId", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        #endregion
    }
}
