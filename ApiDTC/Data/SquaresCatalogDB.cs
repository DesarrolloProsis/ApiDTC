namespace ApiDTC.Data
{
    using ApiDTC.Models;
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
        #endregion
        
        #region Constructor
        public SquaresCatalogDb(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods
        //Test SquaresCatalog
        public Response GetSquaresCatalog()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("Select * From SquaresCatalog", sql);
                    
                    if(sql.State != ConnectionState.Open)
                    {
                        return new Response
                        {
                            Message = "SQL connection is closed",
                            Result = null
                        };
                    }

                    var response = new List<SelectListItem>();
                    var reader = cmd.ExecuteReader();
                    if(reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            response.Add(new SelectListItem
                            {
                                Value = reader["SquareCatalogId"].ToString(),
                                Text = reader["SquareName"].ToString()

                            });
                        }
                        sql.Close();
                        return new Response
                        {
                            Message = "Ok",
                            Result = response
                        };
                    }
                    else
                    {
                        return new Response
                        {
                            Message = "Empty result",
                            Result = null
                        };
                    }
                }
                catch (SqlException ex)
                {
                    return new Response
                    {
                        Message = $"Error: {ex.Message}",
                        Result = null
                    };
                }
            }
        }

        private SquaresCatalog MapToSquaresCatalog(SqlDataReader reader)
        {
            return new SquaresCatalog()
            {
                SquareCatalogId = reader["SquareCatalogId"].ToString(),
                SquareName = reader["SquareName"].ToString(),
                DelegationId = (int)reader["DelegationId"],
            };
        }
        #endregion
    }
}
