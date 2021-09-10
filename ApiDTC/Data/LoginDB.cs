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
    using System.Linq;

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

        public Response GetStoreCookie(UserRefreshToken userRefreshToken)
        {
            try
            {
                List<Cookie> cookies;
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("dbo.spGetSquaresDTC", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userRefreshToken.UserId;

                        cookies = _sqlResult.GetRows<Cookie>("USR", cmd, sql, "GetStoreLogin");
                        if(cookies is null)
                            return new Response { Message = $"Error", Result = null };                        
                    }

                    var token = BuildToken(userRefreshToken.UserId);
                    return new Response{
                        Result = new CookieToken{Cookie = cookies, UserToken = token },
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

        public Response GetStoreLoginInfo(UserRefreshToken userRefreshToken)
        {
            try
            {
                List<Login> loginList;
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("dbo.spGetHeadersDTC", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userRefreshToken.UserId;

                        loginList = _sqlResult.GetRows<Login>("USR", cmd, sql, "GetStoreLogin");
                        if(loginList.Count == 0)
                            return new Response { Message = $"Error", Result = null };                        
                    }
                    return new Response{
                        Result = new LoginValido{LoginList = loginList },
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

        public Response GetStoreLogin(LoginUserInfo loginUserInfo)
        {
            try
            {
                LoginTrue loginTrue;
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
                    return new Response{
                        Result = loginTrue,
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
                expiration = DateTime.UtcNow.AddHours(3);

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
        public Response GetSesionLog(int userId, int pagina, int registros, string nameFilter, string dateFilter)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("dbo.spLoginSesionLogRol", sql);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@NumPagina", SqlDbType.Int).Value = pagina;
                    cmd.Parameters.Add("@RegistroXPagina", SqlDbType.Int).Value = registros;                    
                    AddNullSPParameter(ref cmd, "@DateFilter", SqlDbType.NVarChar, dateFilter);
                    AddNullSPParameter(ref cmd, "@NameFilter", SqlDbType.NVarChar, dateFilter);

                    var ListaSesiones = _sqlResult.GetRows<SessionLogUser>("USR", cmd, sql, "GetSesionLog");

                    if(ListaSesiones.Count == 0){
                        return new Response {
                            Message = "Bad",
                            Result = null
                        };
                    }

                    List<InfoTable> RowSesion = ListaSesiones.Select(item => new InfoTable{
                        FIlaIndex = item.FIlaIndex,
                        Name = item.Name,
                        RollDescription = item.RollDescription,
                        DateStart = item.DateStart
                    }).ToList();

                    PaginacionSesionLog NewPaginaSesion = new PaginacionSesionLog();
                    var totalRegistros = (ListaSesiones[0].FIlaIndex + ListaSesiones[0].FilaIndexAsc) - 1;    
                    decimal totalPag = Convert.ToDecimal((totalRegistros * 1.0) / (registros * 1.0));
                    NewPaginaSesion.NumeroPaginas = (int)Math.Ceiling(totalPag);
                    NewPaginaSesion.PaginaActual = pagina;
                    NewPaginaSesion.RowSesionLog = RowSesion;
                    
                    return new Response {
                        Message = "Ok",
                        Result = NewPaginaSesion
                    };                                                            
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("INIT", ex, "LoginDb: GetSesionLog", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }

        }

        private void AddNullSPParameter<T>(ref SqlCommand SqlCmd, string Name, SqlDbType dbType, T value)
        {            
            if (value ==  null)
                SqlCmd.Parameters.Add(Name, dbType).Value = DBNull.Value;
            else
                SqlCmd.Parameters.Add(Name, dbType).Value = value;
        }

        #endregion
    }
}
