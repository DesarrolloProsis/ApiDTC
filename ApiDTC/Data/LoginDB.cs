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

        private ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public LoginDb(IConfiguration configuration, ApiLogger apiLogger)
        {
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods
        //TODO test Login
        public SqlResult GetTec(string numPlaza)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_TecnicosPlaza", sql))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@PlazaId", SqlDbType.NVarChar).Value = numPlaza;

                        sql.Open();
                        if(sql.State != ConnectionState.Open)
                        {
                            return new SqlResult
                            {
                                Message = "Sql connection is closed",
                                Result = null
                            };
                        }

                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows)
                        {
                            return new SqlResult
                            {
                                Message = "Result not found",
                                Result = null
                            };
                        }

                        var response = new List<SelectListItem>();
                        while (reader.Read())
                        {
                            response.Add(new SelectListItem
                            {
                                //Value = reader["ComponentsStockId"].ToString(),
                                Value = Convert.ToString(reader["UserId"]),
                                Text = Convert.ToString(reader["TecnicosAsignados"])

                            });
                        }
                        sql.Close();
                        return new SqlResult
                        {
                            Message = "Ok",
                            Result = response
                        };
                    }
                    catch (SqlException ex)
                    {
                        _apiLogger.WriteLog(ex, "GetTec");
                        return new SqlResult
                        {
                            Message = $"Error: {ex.Message}",
                            Result = null
                        };
                    }
                }
            }
        }
        public SqlResult GetHeadTec(int idTec)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_LiquidacionTerceroHeader", sql))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@user", SqlDbType.Int).Value = idTec;
                        sql.Open();
                        if(sql.State != ConnectionState.Open)
                        {
                            return new SqlResult
                            {
                                Message = "Sql connection is closed",
                                Result = null
                            };
                        }


                        var response = new List<Login>();
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows)
                        {
                            return new SqlResult
                            {
                                Message = "Result not found",
                                Result = null
                            };
                        }
                        while (reader.Read())
                        {
                            response.Add(MapToLogin(reader));
                        }
                        sql.Close();
                        return new SqlResult
                        {
                            Message = "Ok",
                            Result = response
                        };
                    }
                    catch (SqlException ex)
                    {
                        _apiLogger.WriteLog(ex, "GetHeadTec");
                        return new SqlResult
                        {
                            Message = $"Error: {ex.Message}",
                            Result = null
                        };
                    }
                }
            }
        }

        public SqlResult GetStoreLogin(string nombreUsuario, string passWord, bool flag)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_Login", sql))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar).Value = nombreUsuario;
                        cmd.Parameters.Add("@Contraseña", SqlDbType.NVarChar).Value = passWord;
                        cmd.Parameters.Add("@Flag", SqlDbType.Bit).Value = flag;
                        
                        sql.Open();
                        if(sql.State != ConnectionState.Open)
                        {
                            return new SqlResult
                            {
                                Message = "Sql connection is closed",
                                Result = null
                            };
                        }


                        var response = new List<Login>();
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows)
                        {
                            return new SqlResult
                            {
                                Message = "Result not found",
                                Result = null
                            };
                        }
                        while (reader.Read())
                        {
                            response.Add(MapToLogin(reader));
                        }
                        sql.Close();
                        return new SqlResult
                        {
                            Message = "Ok",
                            Result = response
                        };
                    }
                    catch (SqlException ex)
                    {
                        _apiLogger.WriteLog(ex, "GetStoreLogin");
                        return new SqlResult
                        {
                            Message = $"Error: {ex.Message}",
                            Result = null
                        };
                    }
                }
            }
        }
        public SqlResult GetStoreLoginCookie(string nombreUsuario, string passWord, bool flag)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_Login", sql))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar).Value = nombreUsuario;
                        cmd.Parameters.Add("@Contraseña", SqlDbType.NVarChar).Value = passWord;
                        cmd.Parameters.Add("@Flag", SqlDbType.Bit).Value = flag;

                        sql.Open();
                        if(sql.State != ConnectionState.Open)
                        {
                            return new SqlResult
                            {
                                Message = "Sql connection is closed",
                                Result = null
                            };
                        }
                        
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows)
                        {
                            return new SqlResult
                            {
                                Message = "Result not found",
                                Result = null
                            };
                        }
                        var response = new List<Cookie>();
                        while (reader.Read())
                        {
                            response.Add(MapToCookie(reader));
                        }
                        sql.Close();
                        return new SqlResult
                        {
                            Message = "Ok",
                            Result = response
                        };
                    }
                    catch (SqlException ex)
                    {
                        _apiLogger.WriteLog(ex, "GetComponentData");
                        return new SqlResult
                        {
                            Message = $"Error: {ex.Message}",
                            Result = null
                        };
                    }
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
