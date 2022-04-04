namespace ApiDTC.Data
{
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class SquaresCatalogDb
    {
        #region Attributes
        private readonly string _connectionString;

        private readonly SqlResult _sqlResult;

        private readonly ApiLogger _apiLogger;
        #endregion
        
        #region Constructor
        public SquaresCatalogDb(IConfiguration configuration, SqlResult sqlResult, ApiLogger apiLogger)
        {
            _apiLogger = apiLogger;
            _sqlResult = sqlResult;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods
        public Response GetSquaresCatalog(string clavePlaza)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("Select * From SquaresCatalog", sql);
                    return _sqlResult.GetList<SquaresCatalog>(clavePlaza, cmd, sql, "GetSquaresCatalog");
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "SquaresCatalogDb: GetSquaresCatalog", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetLanes(string square)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spSquareLanes", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Square", SqlDbType.NVarChar).Value = square == "1Bi" ? square + "s" : square;

                        var storedResult = _sqlResult.GetList<Lanes>("USR", cmd, sql, "GetLanes");
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
                _apiLogger.WriteLog("USR", ex, "SquaresCatalog: GetLanes", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response UpdateAdminStatus(UpdateAdminStatus updateAdminStatus)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spUpdateStatusAdminCrud", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = updateAdminStatus.Status;
                        cmd.Parameters.Add("@AdminId", SqlDbType.Int).Value = updateAdminStatus.AdminId;
                        var response = _sqlResult.Put("USR", cmd, sql, "UpdateAdminStatus");
                        return new Response
                        {
                            Message = response.SqlMessage,
                            Result = response.SqlResult
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("USR", ex, "SquaresCatalogDb: UpdateAdminStatus", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response UpdateAdmin(UpdateAdmin updateAdmin)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spUpdateAdminSquareCrud", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar).Value = updateAdmin.Nombre;
                        cmd.Parameters.Add("@ApellidoP", SqlDbType.NVarChar).Value = updateAdmin.ApellidoP;
                        cmd.Parameters.Add("@ApellidoM", SqlDbType.NVarChar).Value = updateAdmin.ApellidoM;
                        cmd.Parameters.Add("@Mail", SqlDbType.NVarChar).Value = updateAdmin.Mail;
                        cmd.Parameters.Add("@Plaza", SqlDbType.NVarChar).Value = updateAdmin.Plaza;
                        cmd.Parameters.Add("@AdminId", SqlDbType.Int).Value = updateAdmin.AdminId;
                        cmd.Parameters.Add("@User", SqlDbType.Int).Value = updateAdmin.UserId;
                        var response = _sqlResult.Put("USR", cmd, sql, "UpdateAdmin");
                        return new Response
                        {
                            Message = response.SqlMessage,
                            Result = response.SqlResult
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("USR", ex, "SquaresCatalogDb: UpdateAdmin", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response InsertAdmin(InsertAdmin insertAdmin)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spInsertAdminSquareCrud", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar).Value = insertAdmin.Nombre;
                        cmd.Parameters.Add("@ApellidoP", SqlDbType.NVarChar).Value = insertAdmin.ApellidoP;
                        cmd.Parameters.Add("@ApellidoM", SqlDbType.NVarChar).Value = insertAdmin.ApellidoM;
                        cmd.Parameters.Add("@Mail", SqlDbType.NVarChar).Value = insertAdmin.Mail;
                        cmd.Parameters.Add("@Plaza", SqlDbType.NVarChar).Value = insertAdmin.Plaza;
                        
                        var storedResult = _sqlResult.Post("USR", cmd, sql, "InsertAdmin");
                        if (storedResult.SqlResult == null)
                            return new Response { Message = "Error: " + storedResult.SqlMessage, Result = null };
                    }
                }
                return new Response
                {
                    Message = "Ok",
                    Result = insertAdmin
                };
            }
            catch(SqlException ex)
            {
                _apiLogger.WriteLog("USR", ex, "SquaresCatalogDb: InsertAdmin", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetAdmins(int userId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spAdminsSquareCrud", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        var storedResult = _sqlResult.GetList<AdminInfo>("USR", cmd, sql, "GetLanes");
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
                _apiLogger.WriteLog("USR", ex, "SquaresCatalog: GetLanes", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        #endregion
    }
}
