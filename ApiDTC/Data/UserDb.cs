namespace ApiDTC.Data
{
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class UserDb
    {
        private readonly string _connectionString;

        private SqlResult _sqlResult;

        private ApiLogger _apiLogger;

        public UserDb(IConfiguration configuration, ApiLogger apiLogger, SqlResult sqlResult)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }

        public Response GetInfo(UserKey userKey)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.spUsersView", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userKey.Id;
                    cmd.Parameters.Add("@Square", SqlDbType.NVarChar).Value = userKey.Square;
                    return _sqlResult.GetList<UserView>(cmd, sql, "GetInfo");
                }
            }
        }

        public Response NewUser(UserInfo userInfo)
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
                    var reader = _sqlResult.Post(cmd, sql);
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
                        Result = userInfo
                    };
                }
            }
        }

        public Response PutPassword(UserPassword userPassword)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.spUpdatePass", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@IdUser", SqlDbType.Int).Value = userPassword.IdUser;
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = userPassword.Password;

                    var reader = _sqlResult.Put(cmd, sql);
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

        public Response PutUser(UserUpdate userUpdate)
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
                
                    var reader = _sqlResult.Put(cmd, sql);
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

        public Response DeleteUser(UserKey userKey)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.spRevokeUser", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@User", SqlDbType.Int).Value = userKey.Id;
                    var reader = _sqlResult.Post(cmd, sql);
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
    }
}
