

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
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        public Response InsertComent(ActividadCalendario actividad)
        {
             try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString)) 
                { 
                    
                    
                        using (SqlCommand cmd = new SqlCommand("dbo.spCalendarComent", sql))
                        { 
                            cmd.CommandType = CommandType.StoredProcedure;                                                        
                            cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = actividad.SquareId;
                            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = actividad.UserId;                            
                            cmd.Parameters.Add("@Month", SqlDbType.Int).Value = actividad.Month;
                            cmd.Parameters.Add("@Year", SqlDbType.Int).Value = actividad.Year;
                            cmd.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = actividad.Comment;

                        var storedResult = _sqlResult.Post(cmd, sql);
                            if (storedResult.SqlResult == null)
                                return new Response { Message = "No se pudo insertar comentario", Result = null };
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

        public Response InsertActivity(ActividadCalendario actividad)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString)) 
                { 
                    //Cambios Emi
                    int numero_carriles = actividad.CapufeLaneNums.Length == actividad.IdGares.Length ? actividad.CapufeLaneNums.Length : 0;

                    for (int i = 0; i < numero_carriles; i++)
                    {
                        using (SqlCommand cmd = new SqlCommand("dbo.spAddCalenadarDay", sql))
                        { 
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@CapufeLaneNum", SqlDbType.NVarChar).Value = actividad.CapufeLaneNums[i];
                            cmd.Parameters.Add("@IdGare", SqlDbType.NVarChar).Value = actividad.IdGares[i];
                            cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = actividad.SquareId;
                            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = actividad.UserId;
                            cmd.Parameters.Add("@Day", SqlDbType.Int).Value = actividad.Day;
                            cmd.Parameters.Add("@Month", SqlDbType.Int).Value = actividad.Month;
                            cmd.Parameters.Add("@Year", SqlDbType.Int).Value = actividad.Year;
                            cmd.Parameters.Add("@FrequencyId", SqlDbType.Int).Value = actividad.FrequencyId;                     
                            cmd.Parameters.Add("@FinalFlag", SqlDbType.Bit).Value = false;
                            cmd.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = ".";
                            
                            cmd.Parameters.Add("@UpdateFlag", SqlDbType.Bit).Value = false;
                            var storedResult = _sqlResult.Post(cmd, sql);
                            if (storedResult.SqlResult == null)
                                return new Response { Message = "No se pudo insertar Actividad en carril" + actividad.CapufeLaneNums[i] + "con idGare" + actividad.IdGares[i], Result = null };
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

        public Response UpdateActivity(ActividadCalendario actividad)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {

                    //Cambios Emi
                    int numero_carriles = actividad.CapufeLaneNums.Length == actividad.IdGares.Length
                    ? actividad.CapufeLaneNums.Length : 0;

                    for (int i = 0; i < numero_carriles; i++)
                    {
                        using (SqlCommand cmd = new SqlCommand("dbo.spAddCalenadarDay", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@CapufeLaneNum", SqlDbType.NVarChar).Value = actividad.CapufeLaneNums[i];
                            cmd.Parameters.Add("@IdGare", SqlDbType.NVarChar).Value = actividad.IdGares[i];
                            cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = actividad.SquareId;
                            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = actividad.UserId;
                            cmd.Parameters.Add("@Day", SqlDbType.Int).Value = actividad.Day;
                            cmd.Parameters.Add("@Month", SqlDbType.Int).Value = actividad.Month;
                            cmd.Parameters.Add("@Year", SqlDbType.Int).Value = actividad.Year;
                            cmd.Parameters.Add("@FrequencyId", SqlDbType.Int).Value = actividad.FrequencyId;                           
                            cmd.Parameters.Add("@FinalFlag", SqlDbType.Bit).Value = false;
                            cmd.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = ".";                                                     
                            cmd.Parameters.Add("@UpdateFlag", SqlDbType.Bit).Value = true;
                            var storedResult = _sqlResult.Post(cmd, sql);
                            if (storedResult.SqlResult == null)
                                return new Response { Message = "No se pudo insertar Actividad en carril" + actividad.CapufeLaneNums[i] + "con idGare" + actividad.IdGares[i], Result = null };
                        }
                    }

                }
                return new Response
                {
                    Message = "Ok",
                    Result = actividad
                };
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(ex, "InsertActivity");
                return new Response { Message = ex.Message, Result = null };
            }

        }

        public DataSet GetStorePdf(int month, int year, int userId, string squareId)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("spCalendarQuery", sql))
                {

                    try
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();


                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@SuqareId", SqlDbType.NVarChar).Value = squareId;
                        cmd.Parameters.Add("@Month", SqlDbType.Int).Value = month;
                        cmd.Parameters.Add("@Year", SqlDbType.Int).Value = year;

                        sql.Open();
                        sqlDataAdapter = new SqlDataAdapter(cmd);
                        sqlDataAdapter.Fill(dataSet);

                        sql.Close();

                        return dataSet;
                    }
                    catch (Exception ex)
                    {
                        _apiLogger.WriteLog(ex, "GetStorePDF");
                        return null;
                    }
                }
            }
        }



        public Response GetStoreFrontLane(ActividadMesYear actividad)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("spCalendarQueryFrontLanes", sql))
                {

                    try
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();


                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = actividad.UserId;
                        cmd.Parameters.Add("@SuqareId", SqlDbType.NVarChar).Value = actividad.SquareId;
                        cmd.Parameters.Add("@Month", SqlDbType.Int).Value = actividad.Month;
                        cmd.Parameters.Add("@Year", SqlDbType.Int).Value = actividad.Year;

                        sql.Open();
                        sqlDataAdapter = new SqlDataAdapter(cmd);
                        sqlDataAdapter.Fill(dataSet);

                        sql.Close();

                        
                        return new Response
                        {
                            Message = "Ok",
                            Result = dataSet
                        };
                    }
                    catch (Exception ex)
                    {
                        _apiLogger.WriteLog(ex, "GetStorePDF");
                        return new Response
                        {
                            Message = "Ok",
                            Result = null
                        };
                    }
                }
            }
        }





        public Response GetStoreFrontComment(ActividadMesYear actividad)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("spCalendarQueryFrontComment", sql))
                {

                    try
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();


                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = actividad.UserId;
                        cmd.Parameters.Add("@SuqareId", SqlDbType.NVarChar).Value = actividad.SquareId;
                        cmd.Parameters.Add("@Month", SqlDbType.Int).Value = actividad.Month;
                        cmd.Parameters.Add("@Year", SqlDbType.Int).Value = actividad.Year;

                        sql.Open();
                        sqlDataAdapter = new SqlDataAdapter(cmd);
                        sqlDataAdapter.Fill(dataSet);

                        sql.Close();

                        return new Response
                        {
                            Message = "Ok",
                            Result = dataSet
                        };
                    }
                    catch (Exception ex)
                    {
                        _apiLogger.WriteLog(ex, "GetStorePDF");
                        return new Response
                        {
                            Message = "Ok",
                            Result = null
                        };
                    }
                }
            }
        }



        public Response GetActivity(ActividadMesYear actividad)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("dbo.spCalendarQuery", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = actividad.UserId;                                                
                        cmd.Parameters.Add("@SuqareId", SqlDbType.NVarChar).Value = actividad.SquareId;
                        cmd.Parameters.Add("@Month", SqlDbType.Int).Value = actividad.Month;
                        cmd.Parameters.Add("@Year", SqlDbType.Int).Value = actividad.Year;
                        sql.Open();

                        //return _sqlResult.GetList<CalendarQuery>(cmd, sql);

                        List<CalendarQuery> lista = new List<CalendarQuery>();
                        var str = cmd.ExecuteReader();

                        while (str.Read())
                        {
                            lista.Add(new CalendarQuery
                            {
                                Lane = str[0].ToString(),
                                CapufeLaneNum = str[1].ToString(),
                                IdGare = str[2].ToString(),
                                Day = str[3].ToString(),
                                FrequencyId = str[4].ToString(),
                                DateStamp = str[5].ToString()
                            });
                        }


                        return new Response
                        {
                            Message = "Ok",
                            Result = lista
                        };
                 
                    }
                }
             
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(ex, "InsertActivity");
                return new Response { Message = ex.Message, Result = null };
            }

        }
    }
}
