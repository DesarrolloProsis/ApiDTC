using ApiDTC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Data
{
    public class TypeDescriptionsDb
    {
        private readonly string _connectionString;


        public TypeDescriptionsDb(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }


        public List<SelectListItem> GetTypeDescriptionsData()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {

                using (SqlCommand cmd = new SqlCommand("", sql))
                {
                    try
                    {

                        string query = string.Empty;
                        query = "Select * From TypeDescriptions";
                        sql.Open();
                        cmd.CommandText = query;

                        var response = new List<SelectListItem>();

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            response.Add(new SelectListItem
                            {
                                Value = reader["TypeDescriptionId"].ToString(),
                                Text = reader["Description"].ToString()

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

        private TypeDescriptions MapToTypeDescriptions(SqlDataReader reader)
        {
            return new TypeDescriptions()
            {
                TypeDescriptionId = (int)reader["TypeDescriptionId"],
                Description = reader["Description"].ToString(),
            };
        }

    }
}
