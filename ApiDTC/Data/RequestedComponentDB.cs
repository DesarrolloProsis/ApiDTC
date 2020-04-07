using ApiDTC.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Data
{
    public class RequestedComponentDb
    {
        private readonly string _connectionString;


        public RequestedComponentDb(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }

        public object PostRequestedComponent(List<RequestedComponent> requestedComponent)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {

                             
                    try
                    {
                        string query = string.Empty;                        
                        
                        sql.Open();
                        int conteo = 1;
                         
                        foreach (var item in requestedComponent)
                        {

                            for (int i = 0; i < item.CapufeLaneNum.Length; i++)
                            {
                                SqlCommand cmd = new SqlCommand("sp_InsertComponents", sql);
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add("@intType", SqlDbType.Int).Value = 1;
                                cmd.Parameters.Add("@intComponentStockId", SqlDbType.Int).Value = item.ComponentsStockId;
                                cmd.Parameters.Add("@strReferenceNumber", SqlDbType.NVarChar).Value = item.ReferenceNumber;
                                cmd.Parameters.Add("@datetimeDate", SqlDbType.DateTime).Value = DateTime.Now;
                                cmd.Parameters.Add("@strBrand", SqlDbType.NChar).Value = item.Marca;
                                cmd.Parameters.Add("@strModel", SqlDbType.NChar).Value = item.Modelo;

                                cmd.Parameters.Add("@strCapufeLaneNum", SqlDbType.NVarChar).Value = item.CapufeLaneNum[i];
                                cmd.Parameters.Add("@strIdGare", SqlDbType.NVarChar).Value = item.IdGare[i];
                                cmd.Parameters.Add("@strSerialNumber", SqlDbType.NVarChar).Value = item.NumSerie[i];

                                cmd.Parameters.Add("@strUnity", SqlDbType.NVarChar).Value = item.Unity;
                                cmd.Parameters.Add("@dateInstallationDate", SqlDbType.DateTime).Value = item.dateInstallationDate;
                                cmd.Parameters.Add("@dateMaintenanceDate", SqlDbType.DateTime).Value = item.dateMaintenanceDate;
                                cmd.Parameters.Add("@intLifeTimeExpected", SqlDbType.Int).Value = item.intLifeTimeExpected;
                                cmd.Parameters.Add("@dateLifeTimeReal", SqlDbType.DateTime).Value = item.dateLifeTimeReal;

                                cmd.Parameters.Add("@intPartida", SqlDbType.Int).Value = conteo;

                            

                            cmd.ExecuteNonQuery();
                            }
                        conteo++;
                    }

                    conteo = 1;
                    foreach (var item in requestedComponent)
                    {

                        for (int i = 0; i < item.CapufeLaneNum.Length; i++)
                        {
                            SqlCommand cmd = new SqlCommand("sp_InsertComponents", sql);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add("@intType", SqlDbType.Int).Value = 2;
                            cmd.Parameters.Add("@intComponentStockId", SqlDbType.Int).Value = item.ComponentsStockId;
                            cmd.Parameters.Add("@strReferenceNumber", SqlDbType.NVarChar).Value = item.ReferenceNumber;
                            cmd.Parameters.Add("@datetimeDate", SqlDbType.DateTime).Value = DateTime.Now;
                            cmd.Parameters.Add("@strBrand", SqlDbType.NChar).Value = item.Marca;
                            cmd.Parameters.Add("@strModel", SqlDbType.NChar).Value = item.Modelo;

                            cmd.Parameters.Add("@strCapufeLaneNum", SqlDbType.NVarChar).Value = item.CapufeLaneNum[i];
                            cmd.Parameters.Add("@strIdGare", SqlDbType.NVarChar).Value = item.IdGare[i];
                            cmd.Parameters.Add("@strSerialNumber", SqlDbType.NVarChar).Value = item.NumSerie[i];

                            cmd.Parameters.Add("@strUnity", SqlDbType.NVarChar).Value = item.Unity;
                            cmd.Parameters.Add("@dateInstallationDate", SqlDbType.DateTime).Value = item.dateInstallationDate;
                            cmd.Parameters.Add("@dateMaintenanceDate", SqlDbType.DateTime).Value = item.dateMaintenanceDate;
                            cmd.Parameters.Add("@intLifeTimeExpected", SqlDbType.Int).Value = item.intLifeTimeExpected;
                            cmd.Parameters.Add("@dateLifeTimeReal", SqlDbType.DateTime).Value = item.dateLifeTimeReal;

                            cmd.Parameters.Add("@intPartida", SqlDbType.Int).Value = conteo;
                            

                            cmd.ExecuteNonQuery();
                        }
                        conteo++;
                    }

                    return null;

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\nMessage ---\n{0}", ex.Message);
                        var error = ex.Message;
                        return error;
                    }
                    finally
                    {
                        sql.Close();
                    }
                
            }
        }

        //private RequestedComponent MapToRequestedComponent(SqlDataReader reader)
        //{
        //    return new RequestedComponent()
        //    {
        //        RequestedComponentId = (int)reader["RequestedComponentId"],
        //        ComponentsStockId = (int)reader["ComponentsStockId"],
        //        ReferenceNumber = reader["ReferenceNumber"].ToString(),
        //        CapufeLaneNum = reader["CapufeLaneNum"].ToString(),
        //        IdGare = reader["IdGare"].ToString(),
        //        RequestDate = Convert.ToDateTime(reader["RequestDate"].ToString()),
        //    };
        //}
    }
}
