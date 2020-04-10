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

                    return _sqlResult.GetList<TecnicosPlaza>(cmd, sql);                    
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
                    
                    return _sqlResult.GetList<Login>(cmd, sql);
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
                    
                    return _sqlResult.GetList<Login>(cmd, sql);
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

                    return _sqlResult.GetList<Cookie>(cmd, sql);
                }
            }
        }

        private Login MapToLogin(SqlDataReader reader)
        {
            return new Login()
            {
                UserId = (int)reader["UserId"],
                AgremmentInfoId = (int)reader["AgremmentInfoId"],
                Nombre = reader["Nombre"].ToString(),
                Plaza = reader["Plaza"].ToString(),
                Agrement = reader["Agrement"].ToString(),
                ManagerName = reader["ManagerName"].ToString(),
                Position = reader["Position"].ToString(),
                Mail = reader["Mail"].ToString(),
                AgremmentDate = Convert.ToDateTime(reader["AgremmentDate"].ToString()),
                DelegationName = reader["DelegationName"].ToString(),
                RegionalCoordination = reader["RegionalCoordination"].ToString(),
            };
        }

        private Cookie MapToCookie(SqlDataReader reader)
        {
            return new Cookie()
            {
                UserId =  (int)reader["UserId"],
                SquareCatalogId = Convert.ToString(reader["SquareCatalogId"]),
                RollId = (int)reader["RollId"]
            };
        }
        #endregion
    }
}
