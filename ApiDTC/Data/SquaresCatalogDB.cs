using ApiDTC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Data
{
    public class SquaresCatalogDB
    {
        private readonly string _connectionString;

        public SquaresCatalogDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }


        public List<SelectListItem> GetSquaresCatalog()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {

                using (SqlCommand cmd = new SqlCommand("", sql))
                {
                    try
                    {
                        string query = string.Empty;
                        query = "Select * From SquaresCatalog";
                        sql.Open();
                        cmd.CommandText = query;

                        var response = new List<SelectListItem>();

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            response.Add(new SelectListItem
                            {
                                Value = reader["SquareCatalogId"].ToString(),
                                Text = reader["SquareName"].ToString()

                            });
                        }
                        return response;

                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                    finally
                    {
                        sql.Close();
                    }
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
    }
}
