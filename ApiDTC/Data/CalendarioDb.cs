

namespace ApiDTC.Data
{
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;

    public class CalendarioDb
    {
        #region Attributes
        private readonly string _connectionString;

        private SqlResult _sqlResult;

        private ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public CalendarioDb(IConfiguration configuration, SqlResult sqlResult, ApiLogger apiLogger)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("preventivoConnection");
        }

        public Response InsertActivity(ActividadCalendario actividad)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    foreach (var capufeLaneNum in actividad.CapufeLaneNums)
                    {
                        foreach (var idGare in actividad.IdGares)
                        {
                            using (SqlCommand cmd = new SqlCommand("dbo.spAddCalenadarDay", sql))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@CapufeLaneNum", SqlDbType.NVarChar).Value = capufeLaneNum;
                                cmd.Parameters.Add("@IdGare", SqlDbType.NVarChar).Value = idGare;
                                cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = actividad.SquareId;
                                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = actividad.UserId;
                                cmd.Parameters.Add("@Day", SqlDbType.Int).Value = actividad.Day;
                                cmd.Parameters.Add("@Month", SqlDbType.Int).Value = actividad.Month;
                                cmd.Parameters.Add("@Year", SqlDbType.Int).Value = actividad.Year;
                                cmd.Parameters.Add("@FrequencyId", SqlDbType.Int).Value = actividad.FrequencyId;
                                if (capufeLaneNum == actividad.CapufeLaneNums.Last() && idGare == actividad.IdGares.Last())
                                {
                                    cmd.Parameters.Add("@FinalFlag", SqlDbType.Bit).Value = true;
                                    cmd.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = actividad.Comment;
                                }
                                else
                                {
                                    cmd.Parameters.Add("@FinalFlag", SqlDbType.Bit).Value = false;
                                    cmd.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = ".";
                                }




                                var storedResult = _sqlResult.Post(cmd, sql);
                                if (storedResult.SqlResult == null)
                                    return new Response { Message = "No se pudo insertar Actividad en " + idGare, Result = null };
                            }
                        }
                    }
                }
                return new Response
                {
                    Message = "Ok",
                    Result = actividad
                };
            }
            catch(SqlException ex)
            {
                _apiLogger.WriteLog(ex, "InsertActivity");
                return new Response { Message = ex.Message, Result = null };
            }
            
        }
        #endregion
    }
}
