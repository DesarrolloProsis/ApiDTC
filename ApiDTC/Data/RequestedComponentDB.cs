namespace ApiDTC.Data
{
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    public class RequestedComponentDb
    {
        #region Attributes
        private readonly string _connectionString;
        
        private SqlResult _sqlResult;
        #endregion

        #region Constructor
        public RequestedComponentDb(IConfiguration configuration, SqlResult sqlResult)
        {
            _sqlResult = sqlResult;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        public InsertResponse PostRequestedComponent(List<RequestedComponent> requestedComponent)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                var result = new InsertResponse();
                foreach (var item in requestedComponent)
                {
                    SqlCommand cmd = new SqlCommand("dbo.sp_InsertComponents", sql);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@intType", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@intComponentStockId", SqlDbType.Int).Value = item.ComponentsStockId;
                    cmd.Parameters.Add("@strReferenceNumber", SqlDbType.NVarChar).Value = item.ReferenceNumber;
                    cmd.Parameters.Add("@datetimeDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@strBrand", SqlDbType.NChar).Value = item.Marca;
                    cmd.Parameters.Add("@strModel", SqlDbType.NChar).Value = item.Modelo;
                    cmd.Parameters.Add("@strCapufeLaneNum", SqlDbType.NVarChar).Value = item.CapufeLaneNum;
                    cmd.Parameters.Add("@strIdGare", SqlDbType.NVarChar).Value = item.IdGare;
                    cmd.Parameters.Add("@strSerialNumber", SqlDbType.NVarChar).Value = item.NumSerie;
                    cmd.Parameters.Add("@strUnity", SqlDbType.NVarChar).Value = item.Unity;
                    cmd.Parameters.Add("@dateInstallationDate", SqlDbType.DateTime).Value = item.DateInstallationDate;
                    cmd.Parameters.Add("@dateMaintenanceDate", SqlDbType.DateTime).Value = item.DateMaintenanceDate;
                    cmd.Parameters.Add("@intLifeTimeExpected", SqlDbType.Int).Value = item.IntLifeTimeExpected;
                    cmd.Parameters.Add("@dateLifeTimeReal", SqlDbType.DateTime).Value = item.DateLifeTimeReal;
                    cmd.Parameters.Add("@intPartida", SqlDbType.Int).Value = item.IntPartida;
                    cmd.Parameters.Add("@strMaintenanceFolio", SqlDbType.NVarChar).Value = item.MaintenanceFolio;

                    result = _sqlResult.Post(cmd, sql);
                    if (result.SqlResult == null)
                    {
                        result.SqlMessage = $"{result.SqlMessage}. No se pudo insertar la partida del modelo {item.Modelo} tipo 1.";
                        return result;
                    }
                }
                foreach (var item in requestedComponent)
                {
                    SqlCommand cmd = new SqlCommand("dbo.sp_InsertComponents", sql);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@intType", SqlDbType.Int).Value = 2;
                    cmd.Parameters.Add("@intComponentStockId", SqlDbType.Int).Value = item.ComponentsStockId;
                    cmd.Parameters.Add("@strReferenceNumber", SqlDbType.NVarChar).Value = item.ReferenceNumber;
                    cmd.Parameters.Add("@datetimeDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@strBrand", SqlDbType.NChar).Value = item.Marca;
                    cmd.Parameters.Add("@strModel", SqlDbType.NChar).Value = item.Modelo;
                    cmd.Parameters.Add("@strCapufeLaneNum", SqlDbType.NVarChar).Value = item.CapufeLaneNum;
                    cmd.Parameters.Add("@strIdGare", SqlDbType.NVarChar).Value = item.IdGare;
                    cmd.Parameters.Add("@strSerialNumber", SqlDbType.NVarChar).Value = item.NumSerie;
                    cmd.Parameters.Add("@strUnity", SqlDbType.NVarChar).Value = item.Unity;
                    cmd.Parameters.Add("@dateInstallationDate", SqlDbType.DateTime).Value = item.DateInstallationDate;
                    cmd.Parameters.Add("@dateMaintenanceDate", SqlDbType.DateTime).Value = item.DateMaintenanceDate;
                    cmd.Parameters.Add("@intLifeTimeExpected", SqlDbType.Int).Value = item.IntLifeTimeExpected;
                    cmd.Parameters.Add("@dateLifeTimeReal", SqlDbType.DateTime).Value = item.DateLifeTimeReal;
                    cmd.Parameters.Add("@intPartida", SqlDbType.Int).Value = item.IntPartida;
                    cmd.Parameters.Add("@strMaintenanceFolio", SqlDbType.NVarChar).Value = item.MaintenanceFolio;

                    //TODO test components insert
                    result = _sqlResult.Post(cmd, sql);
                    if (result.SqlResult == null)
                    {
                        result.SqlMessage = $"{result.SqlMessage}. No se pudo insertar la partida del modelo {item.Modelo} tipo 2.";
                        return result;
                    }
                }
                return result;               
            }
        }
    }
}
