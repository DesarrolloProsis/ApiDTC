namespace ApiDTC.Data
{
    using ApiDTC.Models;
    using ApiDTC.Services;
    using ApiDTC.Utilities;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Collections.Generic;

    public class UserDb
    {
        #region Attributes

        private readonly string _connectionString;

        private readonly SqlResult _sqlResult;

        private readonly ApiLogger _apiLogger;

        #endregion Attributes

        #region Constructor

        public UserDb(IConfiguration configuration, ApiLogger apiLogger, SqlResult sqlResult)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }

        #endregion Constructor

        #region Methods

        public Response GetInfo(UserKey userKey)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spUsersView", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userKey.Id;
                        cmd.Parameters.Add("@Square", SqlDbType.NVarChar).Value = userKey.Square;
                        return _sqlResult.GetList<UserView>("USR", cmd, sql, "GetInfo");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("USR", ex, "UserDb: GetInfo", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response NewUser(UserInfo userInfo)
        {
            Response resp = new Response();
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spAddUser", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = userInfo.Name;
                        cmd.Parameters.Add("@LastName1", SqlDbType.NVarChar).Value = userInfo.LastName1;
                        cmd.Parameters.Add("@LastName2", SqlDbType.NVarChar).Value = userInfo.LastName2;
                        cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = userInfo.Password;
                        cmd.Parameters.Add("@Rol", SqlDbType.Int).Value = userInfo.Rol;
                        sql.Open();
                        var reader = cmd.ExecuteReader();
                        if (!reader.HasRows)
                        {
                            resp.Message = "Fail";
                            resp.Result = null;
                        }
                        else
                        {
                            NuevoUsuario usuarioNuevo = new NuevoUsuario();
                            while (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    usuarioNuevo.SqlResult = reader.GetString(0);
                                    usuarioNuevo.SqlMessage = reader.GetString(1);
                                    usuarioNuevo.UserId = Convert.ToInt32(reader.GetDecimal(2));
                                    usuarioNuevo.Pass = reader.GetString(3);
                                    usuarioNuevo.UserName = reader.GetString(4);
                                }
                                reader.NextResult();
                            }
                            resp.Message = "Ok";
                            resp.Result = usuarioNuevo;
                        }

                        sql.Close();

                        return resp;
                    }
                }
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog("USR", ex, "UserDb: NewUser", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response PutPassword(UserPassword userPassword)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spUpdatePass", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@IdUser", SqlDbType.Int).Value = userPassword.IdUser;
                        cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = userPassword.Password;

                        var reader = _sqlResult.Put("USR", cmd, sql, "PutPassword");
                        if (reader.SqlResult == null)
                        {
                            return new Response
                            {
                                Message = "Fail",
                                Result = null
                            };
                        }
                        return new Response
                        {
                            Message = "Ok",
                            Result = userPassword
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("USR", ex, "UserDb: PutPassword", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response PutUser(UserUpdate userUpdate)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spUpdateUser", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userUpdate.UserId;
                        cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = userUpdate.UserName;
                        cmd.Parameters.Add("@LastName1", SqlDbType.NVarChar).Value = userUpdate.LastName1;
                        cmd.Parameters.Add("@LastName2", SqlDbType.NVarChar).Value = userUpdate.LastName2;
                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = userUpdate.Name;
                        cmd.Parameters.Add("@Mail", SqlDbType.NVarChar).Value = userUpdate.Mail;
                        cmd.Parameters.Add("@Rol", SqlDbType.Int).Value = userUpdate.Rol;

                        var reader = _sqlResult.Put("USR", cmd, sql, "PutUser");
                        if (reader.SqlResult == null)
                        {
                            return new Response
                            {
                                Message = "Fail",
                                Result = null
                            };
                        }
                        return new Response
                        {
                            Message = "Ok",
                            Result = userUpdate
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("USR", ex, "UserDb: PutUser", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response RevokeUser(UserKey userKey, bool nuevoStatus)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spRevokeUser", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@User", SqlDbType.Int).Value = userKey.Id;
                        cmd.Parameters.Add("@StatusNew", SqlDbType.Bit).Value = nuevoStatus;
                        var reader = _sqlResult.Post("USR", cmd, sql, "RevokeUser");
                        if (reader.SqlResult == null)
                        {
                            return new Response
                            {
                                Message = "Fail",
                                Result = null
                            };
                        }
                        return new Response
                        {
                            Message = "Ok",
                            Result = userKey
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("USR", ex, "UserDb: DeleteUser", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        //Método para asignarle plaza  a un usuario
        public Response AddSquareToUser(UserSquare userSquare)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                        using (SqlCommand cmd = new SqlCommand("dbo.spAddSquareToUser", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@SquareCatalogId", SqlDbType.NVarChar).Value = userSquare.SquareCatalogId;
                            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userSquare.UserId;
                            sql.Open();
                            var reader = cmd.ExecuteScalar();

                            //var storedResult = _sqlResult.Post(userSquare.clavePlaza, cmd, sql, "AddSquareToUser");
                            //if (storedResult.SqlResult == null)
                            //    return new Response { Message = "Error al insertar"};
                            Console.WriteLine(reader);
                            sql.Close();
                        }
                }
                return new Response
                {
                    Message = "Ok",
                    Result = "Ejecucion Correcta" 
                };
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(userSquare.clavePlaza, ex, "UserDb: AddSquareToUser", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        //Método para activar usuario
        public Response ActivateUser(string clavePlaza, int UserId)
        {
            Response response = new Response();
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spActivateUser", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sql.Open();
                        var reader = cmd.ExecuteScalar();

                        Console.WriteLine(reader);
                        sql.Close();

                        if (Convert.ToInt32(reader) == 1)
                        {
                            response.Message = "Ok";
                            response.Result = "Activacion correcta";
                        }
                        else
                        {
                            response.Message = "Error";
                            response.Result = "Verifique que el usuario exita";
                        }
                    }
                }
                return response;
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "UserDb: AddSquareToUser", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetUserOfSquare(string SquareId)
        {
            try
            {

                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spUserOfSquare", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@squareId", SqlDbType.NVarChar).Value = SquareId;
                        return _sqlResult.GetList<UserOfSquare>("spUserOfSquare", cmd, sql, "GetUserOfSquare");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("GetListComponentStock", ex, "userDbDataSb: GetUserOfSquare", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response UpdateSquares(string clavePlaza, SquareUpdate PlazaUsuario)
        {
            try
            {
                List<string> PlazasNoInsertOrDelete = new List<string>();
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {

                    int numero_plazas = PlazaUsuario.SquareId.Length;

                    for(int i = 0; i < numero_plazas; i++)
                    {
                        using (SqlCommand cmd = new SqlCommand("dbo.UpdateSquareCatalogOfUser", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@flag", SqlDbType.NVarChar).Value = PlazaUsuario.flag;
                            cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = PlazaUsuario.SquareId[i]; //== "1Bi" ? actividad.SquareId + "s" : actividad.SquareId;
                            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = PlazaUsuario.UserId;


                            var storedResult = _sqlResult.Post(clavePlaza, cmd, sql, "InsertActivity");
                            PlazasNoInsertOrDelete.Add(new String(storedResult.SqlMessage.ToCharArray()));

                                
                        }
                    }
                }
                return new Response
                {
                    Message = "Ok",
                    Result = PlazasNoInsertOrDelete
                };
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertActivity", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        #endregion Methods
    }
}