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
        public Response InsertSupervisor(string clavePlaza, InsertUsuarioAnexo insertUsuario)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.InsertSupervisor", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@nombreSupervisor", SqlDbType.NVarChar).Value = insertUsuario.Nombre;
                        cmd.Parameters.Add("@numeroPlaza", SqlDbType.NVarChar).Value = insertUsuario.SquareId;
                        var result = _sqlResult.Post(clavePlaza, cmd, sql, "GetTestigosPlaza");
                        if(result.SqlResult == null)
                        {
                            return new Response
                            {
                                Message = result.SqlMessage,
                                Result = null
                            };
                        }
                        return new Response
                        {
                            Message = result.SqlMessage,
                            Result = insertUsuario
                        };
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
        public Response InsertTestigo(string clavePlaza, InsertUsuarioAnexo insertUsuario)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.InsertTestigo", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@nombreTestigo", SqlDbType.NVarChar).Value = insertUsuario.Nombre;
                        cmd.Parameters.Add("@numeroPlaza", SqlDbType.NVarChar).Value = insertUsuario.SquareId;
                        var result = _sqlResult.Post(clavePlaza, cmd, sql, "GetTestigosPlaza");
                        if (result.SqlResult == null)
                        {
                            return new Response
                            {
                                Message = result.SqlMessage,
                                Result = null
                            };
                        }
                        return new Response
                        {
                            Message = result.SqlMessage,
                            Result = insertUsuario
                        };
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
                    using (SqlCommand cmd = new SqlCommand("dbo.GetCompRequestAnexo", sql))
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
                        cmd.Parameters.Add("@referenceDTC", SqlDbType.NVarChar).Value = referenceNumber;                     
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
                    using (SqlCommand cmd = new SqlCommand("dbo.GetComponentesAnexo", sql))
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
        public Response GetHeaderAnexo(string clavePlaza, string referenceAnexo)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.GetHeaderAnexo", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@referenceAnexo", SqlDbType.NVarChar).Value = referenceAnexo;
                        var historicComponetesoAnexo = _sqlResult.GetRows<AnexoDTCHistorico>(clavePlaza, cmd, sql, "GetHistoricoComponetesAnexo");
                                                                    
                        foreach(var item in historicComponetesoAnexo)
                        {
                            item.FolioOficio = item.FolioOficio == null ? "" : item.FolioOficio;
                            item.Solicitud = item.Solicitud == null ? "" : item.Solicitud;
                        }

                        return new Response
                        {
                            Result = historicComponetesoAnexo,
                            Message = "Header del anexo",
                            Rows = historicComponetesoAnexo.Count

                        };   
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertComment", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        private (string newId, string idUltimaVersion) GenerarAnexoId(string clavePlaza, string referenceDTC, string referenceAnexo, bool isSubAnexo, char tipoAnexo) 
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
                        var response = _sqlResult.GetList<AnexoCount>(clavePlaza, cmd, sql, "GetConteoVersionesAnexo");

                        if(response.Result != null)
                        {
                            var listCount = (List<AnexoCount>)response.Result;

                            if (isSubAnexo)
                            {
                                return (referenceAnexo + '-' + (listCount.Count + 1), listCount.FirstOrDefault().AnexoReference);
                            }
                            else
                            {
                                //return (referenceDTC + '-' + tipoAnexo + (listCount.Count + 1).ToString(), listCount.FirstOrDefault().AnexoReference);
                                return (referenceDTC + '-' + tipoAnexo + (listCount.Count + 1).ToString(), string.Empty);
                            }
                        }
                        else
                        {
                            if (isSubAnexo)
                            {
                                return (referenceAnexo + '-' + '1', referenceAnexo);
                            }
                            else
                            {
                                return (referenceDTC + '-' + tipoAnexo + '1', string.Empty);
                            }
                        }
                    }                    
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertComment", 1);
                return (string.Empty, string.Empty);
            }            
        }
        public Response InsertAnexoDTC(string clavePlaza, bool isSubAnexo, AnexoDTCInsert anexoDTCInsert)
        {
            try
            {
                var valueNewId = GenerarAnexoId(clavePlaza, anexoDTCInsert.DTCReference, anexoDTCInsert.AnexoReference, isSubAnexo, anexoDTCInsert.TipoAnexo);

                if (valueNewId.newId != string.Empty)
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
                            if(valueNewId.idUltimaVersion == string.Empty)
                                cmd.Parameters.Add("@referenceAnexoAnterior", SqlDbType.NVarChar).Value = DBNull.Value;
                            else
                                cmd.Parameters.Add("@referenceAnexoAnterior", SqlDbType.NVarChar).Value = valueNewId.idUltimaVersion;
                            cmd.Parameters.Add("@referenceAnexo", SqlDbType.NVarChar).Value = valueNewId.newId;
                            cmd.Parameters.Add("@fechaApertura", SqlDbType.DateTime).Value = anexoDTCInsert.FechaApertura;
                            if(anexoDTCInsert.FechaCierre == null)
                                cmd.Parameters.Add("@fechaCierre", SqlDbType.DateTime).Value = DBNull.Value;
                            else
                                cmd.Parameters.Add("@fechaCierre", SqlDbType.DateTime).Value = anexoDTCInsert.FechaCierre;


                            if (anexoDTCInsert.FolioOficio == string.Empty)
                                cmd.Parameters.Add("@folioOficio", SqlDbType.NVarChar).Value = DBNull.Value;
                            else
                                cmd.Parameters.Add("@folioOficio", SqlDbType.NVarChar).Value = anexoDTCInsert.FolioOficio;

                            if (anexoDTCInsert.FechaOficioInicio == null)
                                cmd.Parameters.Add("@fechaOficioInicio", SqlDbType.DateTime).Value = DBNull.Value;
                            else
                                cmd.Parameters.Add("@fechaOficioInicio", SqlDbType.DateTime).Value = anexoDTCInsert.FechaOficioInicio;


                            //if (anexoDTCInsert.FechaOficioFin == null)
                            //    cmd.Parameters.Add("@fechaOficioFin", SqlDbType.DateTime).Value = DBNull.Value;
                            //else
                            //    cmd.Parameters.Add("@fechaOficioFin", SqlDbType.DateTime).Value = anexoDTCInsert.FechaOficioFin;

                            if (anexoDTCInsert.Solicitud == string.Empty)
                                cmd.Parameters.Add("@solicitud", SqlDbType.NVarChar).Value = DBNull.Value;
                            else
                                cmd.Parameters.Add("@solicitud", SqlDbType.NVarChar).Value = anexoDTCInsert.Solicitud;


                            if (anexoDTCInsert.FechaSolicitudInicio == null)
                                cmd.Parameters.Add("@fechaSolicitudInicio", SqlDbType.DateTime).Value = DBNull.Value;
                            else
                                cmd.Parameters.Add("@fechaSolicitudInicio", SqlDbType.DateTime).Value = anexoDTCInsert.FechaSolicitudInicio;


                            //if (anexoDTCInsert.FechaSolicitudFin == null)
                            //    cmd.Parameters.Add("@fechaSolicitudFin", SqlDbType.DateTime).Value = DBNull.Value;
                            //else
                            //    cmd.Parameters.Add("@fechaSolicitudFin", SqlDbType.DateTime).Value = anexoDTCInsert.FechaSolicitudFin;
                            cmd.Parameters.Add("@supervisorId", SqlDbType.Int).Value = anexoDTCInsert.SupervisorId;
                            cmd.Parameters.Add("@testigo1", SqlDbType.Int).Value = anexoDTCInsert.Testigo1Id;
                            cmd.Parameters.Add("@testigo2", SqlDbType.Int).Value = anexoDTCInsert.Testigo2Id;
                            cmd.Parameters.Add("@tipoAnexo", SqlDbType.Char).Value = anexoDTCInsert.TipoAnexo;
                            cmd.Parameters.Add("@isSubVersion", SqlDbType.Bit).Value = isSubAnexo;
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


                        foreach (var item in anexoDTCInsert.ComponentesAnexo)
                        {
                            SqlCommand cmd = new SqlCommand("dbo.InsertComponentesAnexo", sql)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@referenceNumber", SqlDbType.NVarChar).Value = anexoDTCInsert.DTCReference;
                            cmd.Parameters.Add("@AnexoId", SqlDbType.NVarChar).Value = valueNewId.newId;
                            cmd.Parameters.Add("@ComponentDTCId", SqlDbType.Int).Value = item.RequestedComponentId;
                            cmd.Parameters.Add("@NumeroSerie", SqlDbType.NVarChar).Value = item.SerialNumber;                  
                            var result = _sqlResult.Post(clavePlaza, cmd, sql, "InsertCompoenteAnexo");

                            if (result.SqlResult == null)
                            {
                                return new Response
                                {
                                    Message = $"{result.SqlMessage}. No se pudo insertar el compoente",
                                    Result = null
                                };
                            }
                        }

                    }
                    return new Response
                    {
                        Message = "Ok",
                        Result = valueNewId.newId,
                        Rows = 1
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

        public DataSet GetAnexoPDF(string referenceAnexo, bool IsSubVersion)
         {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.GetAnexoPdf", sql))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();


                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@referenceAnexo", SqlDbType.NVarChar).Value = referenceAnexo;
                        cmd.Parameters.Add("@isSubVersion", SqlDbType.Bit).Value = IsSubVersion;

                        sql.Open();
                        sqlDataAdapter = new SqlDataAdapter(cmd);
                        sqlDataAdapter.Fill(dataSet);

                        sql.Close();

                        return dataSet;
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(referenceAnexo, ex, "PdfConsultasDb: GetStorePDF", 1);
                return null;
            }
        }

        public DataSet GetAnexoPDFReporteFotografico(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spCalendarReportToPDF", sql))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;

                        sql.Open();
                        sqlDataAdapter = new SqlDataAdapter(cmd);
                        sqlDataAdapter.Fill(dataSet);

                        sql.Close();

                        return dataSet;
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoDb: GetStorePDF", 1);
                return null;
            }
        }
    }
}
