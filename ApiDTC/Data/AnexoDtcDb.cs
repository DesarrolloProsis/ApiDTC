using ApiDTC.Models;
using ApiDTC.Models.AnexoDTC;
using ApiDTC.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Data
{
    public class AnexoDtcDb
    {
        #region Attributes
        private readonly string _connectionString;

        private readonly SqlResult _sqlResult;

        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public AnexoDtcDb(IConfiguration configuration, SqlResult sqlResult, ApiLogger apiLogger)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        public Response GetSupervisores(string clavePlaza, string plazaId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.GetSupervisorAnexoPlaza", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@plazaId", SqlDbType.NVarChar).Value = plazaId;
                        var componentes = _sqlResult.GetList<AnexoUsuarioPlaza>(clavePlaza, cmd, sql, "GetTestigosPlaza");
                        return componentes;
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertComment", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        public Response GetTestigos(string clavePlaza, string plazaId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.GetTestigosPlaza", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@plazaId", SqlDbType.NVarChar).Value = plazaId;
                        var componentes = _sqlResult.GetList<AnexoUsuarioPlaza>(clavePlaza, cmd, sql, "GetTestigosPlaza");
                        return componentes;
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertComment", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        public Response GetComponentAnexo(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.GetComponentesAnexo", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@referenceNumber", SqlDbType.NVarChar).Value = referenceNumber;
                        var componentes = _sqlResult.GetList<ComponentesAnexoValidos>(clavePlaza, cmd, sql, "GetComponentesAnexo");

                        return componentes;
                    }

                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertComment", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        public Response GetHistoricoAnexo(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.GetHistoricoAnexo", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@referenceNumber", SqlDbType.NVarChar).Value = referenceNumber;
                        var historicoAnexo = _sqlResult.GetList<AnexoDTCHistorico>(clavePlaza, cmd, sql, "GetHistoricoAnexo");

                        return historicoAnexo;
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertComment", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        public Response GetHistoricoComponetesAnexo(string clavePlaza, string referenceAnexo)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.GetComponenteHistoricoAnexo", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@referenceAnexo", SqlDbType.NVarChar).Value = referenceAnexo;
                        var historicComponetesoAnexo = _sqlResult.GetList<AnexoHistoricoComponete>(clavePlaza, cmd, sql, "GetHistoricoComponetesAnexo");

                        return historicComponetesoAnexo;
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertComment", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        public string GenerarAnexoId(string clavePlaza, string referenceDTC, string referenceAnexo, bool isSubAnexo, char tipoAnexo) 
        {

            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.GetConteoVersionesAnexos", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@referenceDTC", SqlDbType.NVarChar).Value = referenceDTC;
                        cmd.Parameters.Add("@referenceAnexo", SqlDbType.NVarChar).Value = referenceAnexo;
                        cmd.Parameters.Add("@isSubVersion", SqlDbType.Bit).Value = isSubAnexo;
                        var response = _sqlResult.Count(clavePlaza, cmd, sql, "GetConteoVersionesAnexo");
                        if(response.SqlResult != null)
                        {
                            if (isSubAnexo)
                            {
                                return referenceAnexo + '-' + (Convert.ToInt32(response.SqlResult.ToString()) + 1);
                                
                            }
                            else
                            {
                                return referenceDTC + '-' +tipoAnexo + (Convert.ToInt32(response.SqlResult.ToString()) + 1).ToString();
                            }
                        }
                        else
                            return string.Empty;
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertComment", 1);
                return string.Empty;
            }            
        }
        public Response InsertAnexoDTC(string clavePlaza, bool isSubAnexo, AnexoDTCInsert anexoDTCInsert)
        {
            try
            {
                string nuevoAnexoId = GenerarAnexoId(clavePlaza, anexoDTCInsert.DTCReference, anexoDTCInsert.AnexoReference, isSubAnexo, anexoDTCInsert.TipoAnexo);

                if (nuevoAnexoId != string.Empty)
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        {
                            SqlCommand cmd = new SqlCommand("dbo.InsertHeaderAnexo", sql)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@referenceDTC", SqlDbType.NVarChar).Value = anexoDTCInsert.DTCReference;
                            cmd.Parameters.Add("@referenceAnexo", SqlDbType.NVarChar).Value = nuevoAnexoId;
                            cmd.Parameters.Add("@fechaApertura", SqlDbType.DateTime).Value = anexoDTCInsert.FechaApertura;
                            cmd.Parameters.Add("@fechaCierre", SqlDbType.DateTime).Value = anexoDTCInsert.FechaCierre;
                            cmd.Parameters.Add("@folioOficio", SqlDbType.NVarChar).Value = anexoDTCInsert.FolioOficio;
                            if (anexoDTCInsert.FechaOficioInicio == null)
                                cmd.Parameters.Add("@fechaOficioInicio", SqlDbType.DateTime).Value = DBNull.Value;
                            else
                                cmd.Parameters.Add("@fechaOficioInicio", SqlDbType.DateTime).Value = anexoDTCInsert.FechaOficioInicio;
                            if (anexoDTCInsert.FechaOficioFin == null)
                                cmd.Parameters.Add("@fechaOficioFin", SqlDbType.DateTime).Value = DBNull.Value;
                            else
                                cmd.Parameters.Add("@fechaOficioFin", SqlDbType.DateTime).Value = anexoDTCInsert.FechaOficioFin;
                            cmd.Parameters.Add("@supervisorId", SqlDbType.Int).Value = anexoDTCInsert.SupervisorId;
                            cmd.Parameters.Add("@tipoAnexo", SqlDbType.Char).Value = anexoDTCInsert.TipoAnexo;
                            var result = _sqlResult.Post(clavePlaza, cmd, sql, "InserAnexoHeader");
                            if (result.SqlResult == null)
                            {
                                return new Response
                                {
                                    Message = $"{result.SqlMessage}. No se pudo insertar el ",
                                    Result = null
                                };
                            }
                        }


                        //foreach (var item in anexoDTCInsert.ComponentesAnexo)
                        //{
                        //    SqlCommand cmd = new SqlCommand("dbo.InsertComponentesAnexo", sql)
                        //    {
                        //        CommandType = CommandType.StoredProcedure
                        //    };
                        //    cmd.CommandType = CommandType.StoredProcedure;
                        //    cmd.Parameters.Add("@referenceNumber", SqlDbType.NVarChar).Value = anexoDTCInsert.DTCReference;
                        //    cmd.Parameters.Add("@AnexoId", SqlDbType.NVarChar).Value = anexoDTCInsert.AnexoReference;
                        //    cmd.Parameters.Add("@ComponentDTCId", SqlDbType.Int).Value = item.RequestedComponentId;
                        //    cmd.Parameters.Add("@NumeroSerie", SqlDbType.NVarChar).Value = item.SerialNumber;
                        //    var result = _sqlResult.Post(clavePlaza, cmd, sql, "InsertCompoenteAnexo");

                        //    if (result.SqlResult == null)
                        //    {
                        //        return new Response
                        //        {
                        //            Message = $"{result.SqlMessage}. No se pudo insertar el compoente",
                        //            Result = null
                        //        };
                        //    }
                        //}

                    }
                    return new Response
                    {
                        Message = "Ok",
                        Result = anexoDTCInsert,
                        Rows = anexoDTCInsert.ComponentesAnexo.Count
                    };
                }
                return new Response
                {
                    Message = "No se puedo generar el id del nuevo anexo",
                    Result = null,
                    Rows = 0
                };
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertComment", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
    }
}
