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
        public Response InsertAnexoDTC(string clavePlaza, AnexoDTCInsert anexoDTCInsert)
        {
            try
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
                        cmd.Parameters.Add("@referenceAnexo", SqlDbType.NVarChar).Value = anexoDTCInsert.AnexoReference;
                        cmd.Parameters.Add("@fechaApertura", SqlDbType.DateTime).Value = anexoDTCInsert.FechaApertura;
                        cmd.Parameters.Add("@fechaCierre", SqlDbType.DateTime).Value = anexoDTCInsert.FechaCierre;
                        cmd.Parameters.Add("@supervisorId", SqlDbType.Int).Value = anexoDTCInsert.SupervisorId;
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
                        cmd.Parameters.Add("@AnexoId", SqlDbType.NVarChar).Value = anexoDTCInsert.AnexoReference;
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
                    Result = anexoDTCInsert,
                    Rows = anexoDTCInsert.ComponentesAnexo.Count
                };
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertComment", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public DataSet GetAnexoPDF(string referenceAnexo)
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
    }
}
