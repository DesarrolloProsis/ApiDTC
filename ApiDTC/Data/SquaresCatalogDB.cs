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

    public class SquaresCatalogDb
    {
        #region Attributes
        private readonly string _connectionString;

        private SqlResult _sqlResult;
        #endregion
        
        #region Constructor
        public SquaresCatalogDb(IConfiguration configuration, SqlResult sqlResult)
        {
            _sqlResult = sqlResult;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods
        //Test SquaresCatalog
        public Response GetSquaresCatalog()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Select * From SquaresCatalog", sql);
                return _sqlResult.GetList<SquaresCatalog>(cmd, sql);
            }
        }
        #endregion
    }
}
