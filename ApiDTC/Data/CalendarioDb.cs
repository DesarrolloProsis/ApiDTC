

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

        private readonly SqlResult _sqlResult;

        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public CalendarioDb(IConfiguration configuration, SqlResult sqlResult, ApiLogger apiLogger)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods
        public Response InsertComent(string clavePlaza, InsertCommentCalendar actividad)
        {
             try
             {
                using (SqlConnection sql = new SqlConnection(_connectionString)) 
                {                                         
                        using (SqlCommand cmd = new SqlCommand("dbo.spCalendarComent", sql))
                        { 
                            cmd.CommandType = CommandType.StoredProcedure;                                                        
                            cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = actividad.SquareId == "1Bi" ? actividad.SquareId + "s" : actividad.SquareId;
                            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = actividad.UserId;                            
                            cmd.Parameters.Add("@Month", SqlDbType.Int).Value = actividad.Month;
                            cmd.Parameters.Add("@Year", SqlDbType.Int).Value = actividad.Year;
                            
                            cmd.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = actividad.Comment;
                            
                            var storedResult = _sqlResult.Post(clavePlaza, cmd, sql, "InsertComent");
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
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertComment", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }

        }

        public Response InsertActivity(string clavePlaza, ActividadCalendario actividad)
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
                            cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = actividad.SquareId == "1Bi" ? actividad.SquareId + "s" : actividad.SquareId;
                            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = actividad.UserId;
                            cmd.Parameters.Add("@Day", SqlDbType.Int).Value = actividad.Day;
                            cmd.Parameters.Add("@Month", SqlDbType.Int).Value = actividad.Month;
                            cmd.Parameters.Add("@Year", SqlDbType.Int).Value = actividad.Year;
                            cmd.Parameters.Add("@FrequencyId", SqlDbType.Int).Value = actividad.FrequencyId;  
                                                      
                            
                            var storedResult = _sqlResult.Post(clavePlaza, cmd, sql, "InsertActivity");
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
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertActivity", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
            
        }

        public Response InsertCalendarReportData(string clavePlaza, CalendarReportData calendarReportData)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spInsertCalendarReportData", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = calendarReportData.ReferenceNumber;
                        cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = calendarReportData.SquareId == "1Bi" ? calendarReportData.SquareId + "s" : calendarReportData.SquareId;
                        cmd.Parameters.Add("@CapufeLaneNum", SqlDbType.NVarChar).Value = calendarReportData.CapufeLaneNum;
                        cmd.Parameters.Add("@IdGare", SqlDbType.NVarChar).Value = calendarReportData.IdGare;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = calendarReportData.UserId;
                        cmd.Parameters.Add("@AdminSquare", SqlDbType.Int).Value = calendarReportData.AdminSquare;
                        cmd.Parameters.Add("@ReportDate", SqlDbType.Date).Value = calendarReportData.ReportDate;
                        cmd.Parameters.Add("@Start", SqlDbType.NVarChar).Value = calendarReportData.Start;
                        cmd.Parameters.Add("@End", SqlDbType.NVarChar).Value = calendarReportData.End;
                        cmd.Parameters.Add("@Observations", SqlDbType.NVarChar).Value = calendarReportData.Observations;
                        cmd.Parameters.Add("@CalendarId", SqlDbType.Int).Value = calendarReportData.CalendarId;
                        var storedResult = _sqlResult.Post(clavePlaza, cmd, sql, "InsertCalendarReportData");
                        if (storedResult.SqlResult == null)
                            return new Response { Message = "No se pudo insertar ReportData", Result = null };
                    }
                }
                return new Response
                {
                    Message = "Ok",
                    Result = calendarReportData
                };
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertCalendarReportData", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response InsertCalendarReportActivities(string clavePlaza, int calendarId, List<CalendarActivity> calendarActivities)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    foreach (var calendarActivity in calendarActivities)
                    {
                        using (SqlCommand cmd = new SqlCommand("dbo.spInsertCalendarReportActivities", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = calendarActivity.ReferenceNumber;
                            cmd.Parameters.Add("@ComponentJob", SqlDbType.Int).Value = calendarActivity.ComponentJob;
                            cmd.Parameters.Add("@JobStatus", SqlDbType.Int).Value = calendarActivity.JobStatus;
                            var storedResult = _sqlResult.Post(clavePlaza, cmd, sql, "InsertCalendarReportActivities");
                            if (storedResult.SqlResult == null)
                                return new Response { Message = "No se pudo insertar ReportData: " + storedResult.SqlMessage, Result = null };
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("dbo.spUpdateCalendarStatus", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@CalendarId", SqlDbType.Int).Value = calendarId;
                        var storedResult = _sqlResult.Post(clavePlaza, cmd, sql, "UpdateCalendarStatus");
                        if (storedResult.SqlResult == null)
                            return new Response { Message = "No se pudo insertar ReportData " + storedResult.SqlMessage, Result = null };
                    }
                }
                return new Response
                {
                    Message = "Ok",
                    Result = calendarActivities
                };
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertCalendarReportActivities", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetActivities(string clavePlaza, int roll, int frequency)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spCalendarActivitiesReport ", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Roll", SqlDbType.Int).Value = roll;
                        cmd.Parameters.Add("@Frecuency", SqlDbType.Int).Value = frequency;

                        var storedResult = _sqlResult.GetList<Activities>(clavePlaza, cmd, sql, "GetActivities");
                        if (storedResult.Result == null)
                            return storedResult;

                        return new Response
                        {
                            Message = "Ok",
                            Result = storedResult.Result
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: GetActivities", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response DeleteCalendar(string clavePlaza, int CalendarId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spDeleteCalendarData", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@CalendarId", SqlDbType.Int).Value = CalendarId;
                        
                        var result = _sqlResult.Post(clavePlaza, cmd, sql, "DeleteCalendar");
                        return new Response
                        {
                            Message = result.SqlMessage,
                            Result = result.SqlResult
                        };
                    }
                }
            }
            catch(SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: DeleteCalendar", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public DataSet GetStorePdf(string clavePlaza, int month, int year, int userId, string squareId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spCalendarQuery", sql))
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
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: GetStorePdf", 1);
                return null;
            }
        }

        public Response GetStoreFrontLane(string clavePlaza, ActividadMesYear actividad)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spCalendarQueryFrontLanes", sql))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = actividad.UserId;
                        cmd.Parameters.Add("@SuqareId", SqlDbType.NVarChar).Value = actividad.SquareId == "1Bi" ? actividad.SquareId + "s" : actividad.SquareId;
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
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: GetStoreFrontLane", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetStoreFrontComment(string clavePlaza, ActividadMesYear actividad)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                using (SqlCommand cmd = new SqlCommand("spCalendarQueryFrontComment", sql))
                { 
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = actividad.UserId;
                        cmd.Parameters.Add("@SuqareId", SqlDbType.NVarChar).Value = actividad.SquareId == "1Bi" ? actividad.SquareId + "s" : actividad.SquareId;
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
                    
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: GetStoreFrontComment", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetActivity(string clavePlaza, ActividadMesYear actividad)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spCalendarQuery", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = actividad.UserId;                                                
                        cmd.Parameters.Add("@SuqareId", SqlDbType.NVarChar).Value = actividad.SquareId == "1Bi" ? actividad.SquareId + "s" : actividad.SquareId;
                        cmd.Parameters.Add("@Month", SqlDbType.Int).Value = actividad.Month;
                        cmd.Parameters.Add("@Year", SqlDbType.Int).Value = actividad.Year;
                        sql.Open();

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
                                DateStamp = str[5].ToString(),
                                CalendarId = str[6].ToString(),
                                StatusMaintenance = str[7].ToString()
                            }) ;
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
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: GetActivity", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }

        }
        #endregion
    }
}
