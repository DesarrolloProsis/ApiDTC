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

        private SqlResult _sqlResult;
        #endregion

        #region Constructor
        public TypeDescriptionsDb(IConfiguration configuration, SqlResult sqlResult)
        {
            _sqlResult = sqlResult;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods

        //TODO Test TypeDescriptions
        public Response GetTypeDescriptionsData()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                SqlCommand descriptionsCommand = new SqlCommand("Select * From TypeDescriptions", sql);
                return _sqlResult.GetList<TypeDescriptions>(descriptionsCommand, sql);
            }
        }
        #endregion
    }
}
