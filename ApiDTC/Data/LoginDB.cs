namespace ApiDTC.Data
{
    using Models;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
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
        //TODO test Login
        public Response GetTec(string numPlaza)
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
        
        public Response GetHeadTec(int idTec)
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

        public Response GetStoreLogin(string nombreUsuario, string passWord, bool flag)
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

        public Response GetStoreLoginCookie(string nombreUsuario, string passWord, bool flag)
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
        #endregion
    }
}
