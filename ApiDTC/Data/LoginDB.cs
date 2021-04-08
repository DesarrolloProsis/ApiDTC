namespace ApiDTC.Data
{
    using Models;
    using Microsoft.Extensions.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using ApiDTC.Services;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;

    public class LoginDb
    {
        #region Attributes
        private readonly string _connectionString;

        private readonly SqlResult _sqlResult;

        private string _hash;

        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public LoginDb(IConfiguration configuration, ApiLogger apiLogger, SqlResult sqlResult) 
        {
            this._hash = Convert.ToString(configuration.GetValue<string>("JWT:key"));
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

                        return _sqlResult.GetList<TecnicosPlaza>("USR", cmd, sql, "GetTec");
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
                        var login = _sqlResult.GetRows<Login>("USR", cmd, sql, "GetHeadTec");
                        if(login.Count == 0)
                            return new Response { Message = $"Error", Result = null };
                        var result = new Response{
                            Message = "Ok",
                            Result = login
                        };
                        return result;
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("INIT", ex, "LoginDb: GetHeadTec", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetStoreLogin(LoginUserInfo loginUserInfo)
        {
            try
            {
                LoginTrue loginTrue;
                List<Login> loginList;
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_Login", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar).Value = loginUserInfo.Username;
                        cmd.Parameters.Add("@Contraseña", SqlDbType.NVarChar).Value = loginUserInfo.Password;
                        
                        loginTrue = _sqlResult.GetRow<LoginTrue>("USR", cmd, sql, "GetStoreLogin");
                        if(loginTrue is null)
                            return new Response { Message = $"Error", Result = null };                        
                    }

                    using (SqlCommand cmd = new SqlCommand("dbo.spGetHeadersDTC", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = loginTrue.UserId;

                        loginList = _sqlResult.GetRows<Login>("USR", cmd, sql, "GetStoreLogin");
                        if(loginList.Count == 0)
                            return new Response { Message = $"Error", Result = null };                        
                    }
                    return new Response{
                        Result = new LoginValido{LoginList = loginList, LoginTrue = loginTrue },
                        Message = "Ok"
                    };
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("INIT", ex, "LoginDb: GetHeadTec", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
            
        }

        public Response RefreshToken(UserRefreshToken userRefreshToken)
        {
            var token = BuildToken(userRefreshToken.UserId);
            return new Response { Result = token, Message = "Ok" };
        }

        public Response GetStoreLoginCookie(LoginUserInfo loginUserInfo)
        {
            try
            {
                LoginTrue loginTrue;
                List<Cookie> cookies;
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_Login", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar).Value = loginUserInfo.Username;
                        cmd.Parameters.Add("@Contraseña", SqlDbType.NVarChar).Value = loginUserInfo.Password;

                        loginTrue = _sqlResult.GetRow<LoginTrue>("USR", cmd, sql, "GetStoreLoginCookie");
                        if(loginTrue is null)
                            return new Response { Message = $"Error", Result = null };
                    }

                    using (SqlCommand cmd = new SqlCommand("dbo.spGetHeadersDTC", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = loginTrue.UserId;

                        cookies = _sqlResult.GetRows<Cookie>("USR", cmd, sql, "GetStoreLogin");
                        if(cookies is null)
                            return new Response { Message = $"Error", Result = null };                        
                    }

                    var token = BuildToken(loginTrue.UserId);
                    return new Response{
                        Result = new CookieToken{LoginTrue = loginTrue, Cookie = cookies, UserToken = token },
                        Message = "Ok"
                    };
                    
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("INIT", ex, "LoginDb: GetHeadTec", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        private UserToken BuildToken(int userId)
        {
            var claims = new []
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, Convert.ToString(userId)),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._hash));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expiration = new DateTime();
            if(userId == 40)
                expiration = DateTime.UtcNow.AddDays(1);
            else
                expiration = DateTime.UtcNow.AddMinutes(3);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds);
            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
        #endregion
    }
}
