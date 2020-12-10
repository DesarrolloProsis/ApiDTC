﻿namespace ApiDTC.Data
{
    using Models;
    using Microsoft.Extensions.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using ApiDTC.Services;

    public class LoginDb
    {
        #region Attributes
        private readonly string _connectionString;

        private SqlResult _sqlResult;

        private ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public LoginDb(IConfiguration configuration, ApiLogger apiLogger, SqlResult sqlResult)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods
        public Response GetTec(string numPlaza)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_TecnicosPlaza", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@PlazaId", SqlDbType.NVarChar).Value = numPlaza;

                        return _sqlResult.GetList<TecnicosPlaza>(cmd, sql, "GetTec");
                    }
                }
            }
            catch(SqlException ex)
            {
                _apiLogger.WriteLog("INIT", ex, "LoginDb: GetTec", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
            
        }
        
        public Response GetHeadTec(int idTec)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_LiquidacionTerceroHeader", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@user", SqlDbType.Int).Value = idTec;

                        return _sqlResult.GetList<Login>(cmd, sql, "GetHeadTec");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("INIT", ex, "LoginDb: GetHeadTec", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetStoreLogin(string nombreUsuario, string passWord, bool flag)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_Login", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar).Value = nombreUsuario;
                        cmd.Parameters.Add("@Contraseña", SqlDbType.NVarChar).Value = passWord;
                        cmd.Parameters.Add("@Flag", SqlDbType.Bit).Value = flag;

                        return _sqlResult.GetList<Login>(cmd, sql, "GetStoreLogin");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("INIT", ex, "LoginDb: GetHeadTec", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
            
        }

        public Response GetStoreLoginCookie(string nombreUsuario, string passWord, bool flag)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_Login", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar).Value = nombreUsuario;
                        cmd.Parameters.Add("@Contraseña", SqlDbType.NVarChar).Value = passWord;
                        cmd.Parameters.Add("@Flag", SqlDbType.Bit).Value = flag;

                        return _sqlResult.GetList<Cookie>(cmd, sql, "GetStoreLoginCookie");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("INIT", ex, "LoginDb: GetHeadTec", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        #endregion
    }
}
