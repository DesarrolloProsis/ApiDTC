using ApiDTC.Models;
using ApiDTC.Services;
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
    public class ComponentDB
    {
        #region Attributes
        private readonly string _connectionString;
        private SqlMapper _sqlMapper;
        #endregion

        #region Constructor
        public ComponentDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        
        public SqlResult GetComponentData(string convenio, string plaza, string Id)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_ComponentInfoDTC", sql))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Agremmnt", SqlDbType.NVarChar).Value = convenio;
                        cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = plaza;
                        cmd.Parameters.Add("@Component", SqlDbType.NVarChar).Value = Id;
                        
                        
                        sql.Open();
                        if(sql.State != ConnectionState.Open)
                        {
                            return new SqlResult
                            {
                                Message = "SqlConnection is closed",
                                Result = null
                            };
                        }
                        
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows)
                        {
                            return new SqlResult
                            {
                                Message = "Result not found",
                                Result = null
                            };
                        }

                        var response = new List<Components>();
                        while (reader.Read())
                        {
                            response.Add(MapToComponents(reader));
                        }
                        var limite = response.Count();
                        string[] listLane = new string[limite];

                        //for(int i = 0; i < limite; i++)
                        //{
                        //    listLane[i] = response[i].Lane;
                        //}

                       int i = 0;
                        foreach(var lane in response)
                        {
                            listLane[i++] = lane.Lane;
                        }

                        //object json = new { response, listLane };
                        //return json;
                        //string query = string.Empty;
                
                        return new SqlResult
                            {
                                Message = "Result not found",
                                Result = null
                            };
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\nMessage ---\n{0}", ex.Message);
                        return null;
                    }
                    finally
                    {
                        sql.Close();
                    }
                }
            }
        }
        public List<SelectListItem> GetComponentData1()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {

                using (SqlCommand cmd = new SqlCommand("", sql))
                {
                    try
                    {

                        string query = string.Empty;
                        //Query para saber si existe ReferenceNumber
                        query = "select a.Component as description from SquareInventory a join LanesCatalog b on (a.CapufeLaneNum = b.CapufeLaneNum and a.IdGare = b.IdGare) where b.SquareCatalogId = '102' group by a.Component";
                        sql.Open();
                        cmd.CommandText = query;

                        var response = new List<SelectListItem>();

                        
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            response.Add(new SelectListItem
                            {
                                //Value = reader["ComponentsStockId"].ToString(),
                                Text = reader["Description"].ToString()

                            }) ;
                        }
                        return response;

                    }
                        catch (Exception ex)
                        {
                            Console.WriteLine("\nMessage ---\n{0}", ex.Message);
                            return null;
                        }
                        finally
                        {
                            sql.Close();
                        }
                    }
            }
        }

        private Components MapToComponents(SqlDataReader reader)
        {
            return new Components()
            {
                Unity = reader["Unity"].ToString(),
                Description = reader["Description"].ToString(),
                Brand = reader["Brand"].ToString(),
                Model = reader["Model"].ToString(),
                SerialNumber = reader["SerialNumber"].ToString(),
                InstalationDate = Convert.ToDateTime(reader["InstalationDate"].ToString()),
                LifeTime = (int)reader["LifeTime"],
                Lane = reader["Lane"].ToString(),
                IdGare = reader["IdGare"].ToString(),
                CapufeLaneNum = reader["CapufeLaneNum"].ToString(),
                ComponentsStockId = (int)reader["ComponentsStockId"],
                UnitaryPrice = (decimal)reader["UnitaryPrice"],
                SelfAssignable = (bool)reader["SelfAssignable"],
                VitalComponent = (bool)reader["VitalComponent"],
                CatalogBrand = reader["CatalogBrand"].ToString(),
                CatalogModel = reader["CatalogModel"].ToString()
            };
        }
    }
}
