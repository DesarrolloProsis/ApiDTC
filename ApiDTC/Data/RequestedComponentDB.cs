﻿namespace ApiDTC.Data
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

        public SqlResponse PostRequestedComponent(List<RequestedComponent> requestedComponent, bool flag)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                var result = new SqlResponse();

                foreach (var item in requestedComponent)
                {
                    /* MODO NO LIBRE
                    SqlCommand cmd = new SqlCommand("dbo.sp_InsertComponents", sql);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@intType", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@flag", SqlDbType.Int).Value = flag;
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
                    cmd.Parameters.Add("@strLifeTimeReal", SqlDbType.NVarChar).Value = item.strLifeTimeReal;
                    cmd.Parameters.Add("@intPartida", SqlDbType.Int).Value = item.IntPartida;
                    cmd.Parameters.Add("@strMaintenanceFolio", SqlDbType.NVarChar).Value = item.MaintenanceFolio;
                    
                    if(item == requestedComponent[requestedComponent.Count - 1])
                        cmd.Parameters.Add("@validationFlag", SqlDbType.Bit).Value = 1;
                    else
                        cmd.Parameters.Add("@validationFlag", SqlDbType.Bit).Value = 0;

                    if (item == requestedComponent[0])
                        cmd.Parameters.Add("@startFlag", SqlDbType.Bit).Value = 1;
                    else
                        cmd.Parameters.Add("@startFlag", SqlDbType.Bit).Value = 0;
                    */
                    SqlCommand cmd = new SqlCommand("dbo.spInsertComponentsOpen", sql);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@intType", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@bitflag", SqlDbType.Bit).Value = flag;
                    cmd.Parameters.Add("@strReferenceNumber", SqlDbType.NVarChar).Value = item.StrReferenceNumber;
                    cmd.Parameters.Add("@strUnity", SqlDbType.NVarChar).Value = item.StrUnity;
                    cmd.Parameters.Add("@strComponent", SqlDbType.NVarChar).Value = item.StrComponent;
                    cmd.Parameters.Add("@strBrand", SqlDbType.NChar).Value = item.StrBrand;
                    cmd.Parameters.Add("@strModel", SqlDbType.NVarChar).Value = item.StrModel;
                    cmd.Parameters.Add("@strSerialNumber", SqlDbType.NVarChar).Value = item.StrSerialNumber;
                    cmd.Parameters.Add("@strLane", SqlDbType.NVarChar).Value = item.StrLane;
                    cmd.Parameters.Add("@dateInstallationDate", SqlDbType.DateTime).Value = item.DateInstallationDate;
                    cmd.Parameters.Add("@dateMaintenanceDate", SqlDbType.DateTime).Value = item.DateMaintenanceDate;
                    cmd.Parameters.Add("@strLifeTimeExpected", SqlDbType.NVarChar).Value = item.StrLifeTimeExpected;
                    cmd.Parameters.Add("@intItem", SqlDbType.Int).Value = item.IntItem;
                    cmd.Parameters.Add("@strMaintenanceFolio", SqlDbType.NVarChar).Value = item.StrMaintenanceFolio;
                    cmd.Parameters.Add("@strlifeTimeReal", SqlDbType.NVarChar).Value = item.StrLifeTimeReal;
                    cmd.Parameters.Add("@strUnitaryPrice", SqlDbType.NVarChar).Value = item.StrUnitaryPrice;
                    cmd.Parameters.Add("@strDollarUnitaryPrice", SqlDbType.NVarChar).Value = item.StrDollarUnitaryPrice;
                    cmd.Parameters.Add("@strTotalPrice", SqlDbType.NVarChar).Value = item.StrTotalPrice;
                    cmd.Parameters.Add("@srDollarTotalPrice", SqlDbType.NVarChar).Value = item.StrDollarTotalPrice;
                    result = _sqlResult.Post(cmd, sql);
                    if (result.SqlResult == null)
                    {
                        result.SqlMessage = $"{result.SqlMessage}. No se pudo insertar la partida del modelo {item.StrModel} tipo 1.";
                        return result;
                    }
                }
                foreach (var item in requestedComponent)
                {
                    /*SqlCommand cmd = new SqlCommand("dbo.sp_InsertComponents", sql);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@intType", SqlDbType.Int).Value = 2;
                    cmd.Parameters.Add("@flag", SqlDbType.Int).Value = flag;
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
                    cmd.Parameters.Add("@strLifeTimeReal", SqlDbType.NVarChar).Value = item.strLifeTimeReal;
                    cmd.Parameters.Add("@intPartida", SqlDbType.Int).Value = item.IntPartida;
                    cmd.Parameters.Add("@strMaintenanceFolio", SqlDbType.NVarChar).Value = item.MaintenanceFolio;

                    //TODO test components insert

                    if (item == requestedComponent[requestedComponent.Count - 1])
                        cmd.Parameters.Add("@validationFlag", SqlDbType.Bit).Value = 1;
                    else
                        cmd.Parameters.Add("@validationFlag", SqlDbType.Bit).Value = 0;


                    if (item == requestedComponent[0])
                        cmd.Parameters.Add("@startFlag", SqlDbType.Bit).Value = 1;
                    else
                        cmd.Parameters.Add("@startFlag", SqlDbType.Bit).Value = 0;
                    */
                    SqlCommand cmd = new SqlCommand("dbo.spInsertComponentsOpen", sql);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@intType", SqlDbType.Int).Value = 2;
                    cmd.Parameters.Add("@bitflag", SqlDbType.Bit).Value = flag;
                    cmd.Parameters.Add("@strReferenceNumber", SqlDbType.NVarChar).Value = item.StrReferenceNumber;
                    cmd.Parameters.Add("@strUnity", SqlDbType.NVarChar).Value = item.StrUnity;
                    cmd.Parameters.Add("@strComponent", SqlDbType.NVarChar).Value = item.StrComponent;
                    cmd.Parameters.Add("@strBrand", SqlDbType.NChar).Value = item.StrBrand;
                    cmd.Parameters.Add("@strModel", SqlDbType.NVarChar).Value = item.StrModel;
                    cmd.Parameters.Add("@strSerialNumber", SqlDbType.NVarChar).Value = item.StrSerialNumber;
                    cmd.Parameters.Add("@strLane", SqlDbType.NVarChar).Value = item.StrLane;
                    cmd.Parameters.Add("@dateInstallationDate", SqlDbType.DateTime).Value = item.DateInstallationDate;
                    cmd.Parameters.Add("@dateMaintenanceDate", SqlDbType.DateTime).Value = item.DateMaintenanceDate;
                    cmd.Parameters.Add("@strLifeTimeExpected", SqlDbType.NVarChar).Value = item.StrLifeTimeExpected;
                    cmd.Parameters.Add("@intItem", SqlDbType.Int).Value = item.IntItem;
                    cmd.Parameters.Add("@strMaintenanceFolio", SqlDbType.NVarChar).Value = item.StrMaintenanceFolio;
                    cmd.Parameters.Add("@strlifeTimeReal", SqlDbType.NVarChar).Value = item.StrLifeTimeReal;
                    cmd.Parameters.Add("@strUnitaryPrice", SqlDbType.NVarChar).Value = item.StrUnitaryPrice;
                    cmd.Parameters.Add("@strDollarUnitaryPrice", SqlDbType.NVarChar).Value = item.StrDollarUnitaryPrice;
                    cmd.Parameters.Add("@strTotalPrice", SqlDbType.NVarChar).Value = item.StrTotalPrice;
                    cmd.Parameters.Add("@srDollarTotalPrice", SqlDbType.NVarChar).Value = item.StrDollarTotalPrice;
                    result = _sqlResult.Post(cmd, sql);
                    result = _sqlResult.Post(cmd, sql);
                    if (result.SqlResult == null)
                    {
                        result.SqlMessage = $"{result.SqlMessage}. No se pudo insertar la partida del modelo {item.StrModel} tipo 2.";
                        return result;
                    }
                }
                return result;               
            }
        }
    }
}
