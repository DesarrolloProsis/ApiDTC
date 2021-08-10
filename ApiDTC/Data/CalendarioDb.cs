

namespace ApiDTC.Data
{
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
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
                        cmd.Parameters.Add("@AdminId", SqlDbType.Int).Value = actividad.AdminId;
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

        public Response GetActividadesUsuario(string clavePlaza, ActividadesUsuario actividadesUsuario, string disk, string folder)//
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[dbo].[spCalendarQueryFront]", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = actividadesUsuario.UserId;
                        cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = actividadesUsuario.SquareId;
                        cmd.Parameters.Add("@Year", SqlDbType.Int).Value = actividadesUsuario.Year;
                        cmd.Parameters.Add("@Month", SqlDbType.Int).Value = actividadesUsuario.Month;
                        var storedResult =  _sqlResult.GetRows<CalendarQueryFront>(clavePlaza, cmd, sql, "GetTechnicalSheet");

                        ////
                        List<CalendarQueryFrontInfo> dtcViewInfo = new List<CalendarQueryFrontInfo>();

                        foreach (var dtcView in storedResult)
                        {
                            CalendarQueryFrontInfo viewInfo = new CalendarQueryFrontInfo
                            {
                                Lane = dtcView.Lane,
                                CapufeLaneNum = dtcView.CapufeLaneNum,
                                IdGare= dtcView.IdGare,
                                Day= dtcView.Day,
                                FrequencyId= dtcView.FrequencyId,
                                DateStamp= dtcView.DateStamp,
                                CalendarId= dtcView.CalendarId,
                                StatusMaintenance = dtcView.StatusMaintenance,
                                ReferenceNumber = dtcView.ReferenceNumber,
                            };
                            //D:\BitacoraDesarrollo\TLA\Reportes\TLA-DF-21146
                            string path = $@"{disk}:\{folder}\{dtcView.ReferenceNumber.Split('-')[0].ToUpper()}\Reportes\{dtcView.ReferenceNumber}\{dtcView.ReferenceNumber}-Escaneado.pdf";
                            if (System.IO.File.Exists((path)))
                                viewInfo.PdfExists = true;
                            else
                                viewInfo.PdfExists = false;
                            dtcViewInfo.Add(viewInfo);
                        }
                        return new Response
                        {
                            Result = dtcViewInfo,
                            Message = "Ok"
                        };


                        ///
                        //return new Response
                        //{
                        //    Message = "Ok",
                        //    Result = storedResult.Result
                        //};
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: GetActividadesUsuario", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetActividadesFiltroReferencia(string clavePlaza, ActividadesUsuarioFiltro actividadesUsuarioFiltro)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[dbo].[spCalendarQueryReferenceSearch]", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = actividadesUsuarioFiltro.UserId;
                        cmd.Parameters.Add("@Reference", SqlDbType.NVarChar).Value = actividadesUsuarioFiltro.ReferenceNumber;
                        var storedResult =  _sqlResult.GetList<CalendarQueryReferenceSearch>(clavePlaza, cmd, sql, "GetActividadesFiltroReferencia");
                        if (storedResult.Result is null)
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
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: GetActividadesFiltroReferencia", 1);
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

        public Response InsertCalendarReportData(string clavePlaza, CalendarReportData calendarReportData, bool update)
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
                        cmd.Parameters.Add("@UpdateFlag", SqlDbType.BigInt).Value = update;
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

        public Response InsertCalendarDateLog(string clavePlaza, CalendarDateLog calendarDateLog)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spCalendarDateLog", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = calendarDateLog.ReferenceNumber;
                        cmd.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = (string.IsNullOrEmpty(calendarDateLog.Comment)) ? "." :calendarDateLog.Comment;
                        cmd.Parameters.Add("@CalendarId", SqlDbType.Int).Value = calendarDateLog.CalendarId;
                        cmd.Parameters.Add("@Date", SqlDbType.Date).Value = calendarDateLog.Date;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = calendarDateLog.UserId;
                        var storedResult = _sqlResult.Post(clavePlaza, cmd, sql, "InsertCalendarDateLog");
                        if (storedResult.SqlResult == null)
                            return new Response { Message = "No se pudo insertar ReportData", Result = null };
                    }
                }
                return new Response
                {
                    Message = "Ok",
                    Result = calendarDateLog
                };
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertCalendarReportData", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetCalendarInfo(string clavePlaza, int calendarId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spPreventiveMaintenance", sql))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@CalendarId", SqlDbType.Int).Value = calendarId; 

                        sql.Open();
                        sqlDataAdapter = new SqlDataAdapter(cmd);
                        sqlDataAdapter.Fill(dataSet);
                        sql.Close();

                        if(dataSet.Tables[0].Rows.Count == 0 || dataSet.Tables[1].Rows.Count == 0 )
                            return new Response { Result = null, Message = "Sin resultado"};
                        CalendarInfo calendarInfo = new CalendarInfo
                        {
                            CalendarHeader = _sqlResult.GetRow<CalendarHeader>(clavePlaza, dataSet.Tables[0], "CalendarInfo: CalendarHeader"),
                            ActivitiesDescription = _sqlResult.GetRows<ActivitiesDescription>(clavePlaza, dataSet.Tables[1], "CalendarInfo: ActivitiesDescription")
                        };
                        return new Response { Result = calendarInfo, Message = "OK"};                        
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: GetStorePdf", 1);
                return null;
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
                            cmd.Parameters.Add("@FlagUpdate", SqlDbType.Bit).Value = calendarActivity.FlagUpdate;
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

        public Response GetDataReportEdit(string clavePlaza, int calendarId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spPreventiveMaintenance ", sql))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@CalendarId", SqlDbType.Int).Value = calendarId;

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
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: GetActivities", 1);
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

        public Response GetActivity(string clavePlaza, ActividadMesYear actividad, string disk, string folder)//---- ---------- ------------ ------------- ---------   ----------
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
                        
                        var storedResult = _sqlResult.GetRows<CalendarQuery>(clavePlaza, cmd, sql, "GetActivity");

                        List<CalendarQueryFrontInfo> dtcViewInfo = new List<CalendarQueryFrontInfo>();

                        foreach (var dtcView in storedResult)
                        {
                            CalendarQueryFrontInfo viewInfo = new CalendarQueryFrontInfo
                            {
                                Lane= dtcView.Lane,
                                CapufeLaneNum = dtcView.CapufeLaneNum,
                                IdGare = dtcView.IdGare,
                                Day = dtcView.Day,
                                FrequencyId = dtcView.FrequencyId,
                                DateStamp = dtcView.DateStamp,
                                CalendarId = dtcView.CalendarId,
                                StatusMaintenance = dtcView.StatusMaintenance,
                                ReferenceNumber = dtcView.ReferenceNumber
                            };
                            //D:\BitacoraDesarrollo\TLA\Reportes\TLA-DF-21146
                            string path = $@"{disk}:\{folder}\{dtcView.ReferenceNumber.Split('-')[0].ToUpper()}\Reportes\{dtcView.ReferenceNumber}\{dtcView.ReferenceNumber}-Escaneado.pdf";
                            if (System.IO.File.Exists((path)))
                                viewInfo.PdfExists = true;
                            else
                                viewInfo.PdfExists = false;
                            dtcViewInfo.Add(viewInfo);
                        }
                        return new Response
                        {
                            Result = dtcViewInfo,
                            Message = "Ok"
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

        public DataSet getActividadesPreventivo(string clavePlaza, int userId, string squareId, int year)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spCalendarQueryFront", sql))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = squareId;
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
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: getActividadesPreventivo", 1);
                return null;
            }
        }
        #endregion
    }
}
