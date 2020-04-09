namespace ApiDTC.Data
{
    using Models;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using ApiDTC.Services;

    public class ComponentDb
    {
        #region Attributes
        private readonly string _connectionString;

        private ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public ComponentDb(IConfiguration configuration, ApiLogger apiLogger)
        {
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods
        public OperationResult GetComponentData(string convenio, string plaza, string Id)
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
                            return new OperationResult
                            {
                                Message = "Sql connection is closed",
                                Result = null
                            };
                        }
                        
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows)
                        {
                            return new OperationResult
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

                        int i = 0;
                        foreach(var lane in response)
                        {
                            listLane[i++] = lane.Lane;
                        }

                        object json = new { response, listLane };
                        sql.Close();
                        return new OperationResult
                        {
                            Message = "Ok",
                            Result = json
                        };
                    }
                    catch (SqlException ex)
                    {
                        _apiLogger.WriteLog(ex, "GetComponentData");
                        return new OperationResult
                        {
                            Message = $"Error: {ex.Message}",
                            Result = null
                        };
                    }
                }
            }
        }

        //Revisar
        public OperationResult GetComponentsData()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("select a.Component as description from SquareInventory a join LanesCatalog b on (a.CapufeLaneNum = b.CapufeLaneNum and a.IdGare = b.IdGare) where b.SquareCatalogId = '102' group by a.Component", sql);
                    sql.Open();
                    if(sql.State != ConnectionState.Open)
                    {
                        return new OperationResult
                        {
                            Message = "Sql connection is closed",
                            Result = null
                        };
                    }
                    var response = new List<SelectListItem>();
                    var reader = cmd.ExecuteReader();
                    
                    if(!reader.HasRows)
                    {
                        return new OperationResult
                        {
                            Message = "Result not found",
                            Result = null
                        };
                    }

                    while (reader.Read())
                    {
                        response.Add(new SelectListItem
                        {
                            //Value = reader["ComponentsStockId"].ToString(),
                            Text = reader["Description"].ToString()

                        }) ;
                    }
                    sql.Close();
                    return new OperationResult
                    {
                        Message = "Ok",
                        Result = response
                    };
                }
                catch (SqlException ex)
                {
                    _apiLogger.WriteLog(ex, "GetComponentData");
                    return new OperationResult
                    {
                        Message = $"Error: {ex.Message}",
                        Result = null
                    };
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
        #endregion
    }
}
