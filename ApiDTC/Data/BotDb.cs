using ApiDTC.Models;
using ApiDTC.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Data
{
    public class BotDb
    {
        #region Attributes
        private readonly string _connectionString;

        private SqlResult _sqlResult;

        private ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public BotDb(IConfiguration configuration, SqlResult sqlResult, ApiLogger apiLogger)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        public bool UpdateUserStatus(string Key, int UserId)
        {
            using(SqlConnection sql = new SqlConnection(_connectionString))
            {
                try
                {
                    sql.Open();
                    SqlCommand countCommand = new SqlCommand("SELECT COUNT (*) " +
                        $"FROM [ProsisDTC].[dbo].[KeysInUse] WHERE [Key] = '{Key}'", sql);
                    Int32 count = (Int32)countCommand.ExecuteScalar();
                    if (count == 0)
                    {
                        sql.Close();
                        return false;
                    }
                    else if (count == 1)
                    {
                        SqlCommand deleteCommand = new SqlCommand($"DELETE FROM [ProsisDTC].[dbo].[KeysInUse] WHERE [Key] = '{Key}'", sql);
                        deleteCommand.ExecuteNonQuery();
                        SqlCommand cmd = new SqlCommand("UPDATE [ProsisDTC].[dbo].[DTCUsers] " +
                            "SET [StatusUser] = 1 " +
                            $"WHERE UserId = {UserId}", sql);
                        cmd.ExecuteNonQuery();
                        sql.Close();
                        return true;
                    }
                    else
                    {
                        SqlCommand deleteCommand = new SqlCommand($"DELETE FROM FROM [ProsisDTC].[dbo].[KeysInUse] WHERE [Key] = '{Key}'", sql);
                        deleteCommand.ExecuteNonQuery();
                        sql.Close();
                        return false;
                    }
                }
                catch(SqlException ex)
                {
                    sql.Close();
                    _apiLogger.WriteLog(ex, "UpdateUserStatus");
                    return false;
                }
            }
        }

        public bool DeleteUser(string key, int idUser)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                try
                {
                    sql.Open();
                    SqlCommand countCommand = new SqlCommand($"SELECT COUNT (*)" +
                        $"FROM [ProsisDTC].[dbo].[KeysInUse] WHERE [Key] = '{key}'", sql);
                    Int32 count = (Int32)countCommand.ExecuteScalar();
                    if (count == 0)
                    {
                        sql.Close();
                        return false;
                    }
                    else if (count == 1)
                    {
                        SqlCommand deleteCommand = new SqlCommand($"DELETE FROM [ProsisDTC].[dbo].[KeysInUse] WHERE [Key] = '{key}'", sql);
                        deleteCommand.ExecuteNonQuery();
                        SqlCommand cmd = new SqlCommand("DELETE FROM [ProsisDTC].[dbo].[DTCUsers] " +
                            $"WHERE UserId = {idUser}", sql);
                        cmd.ExecuteNonQuery();
                        sql.Close();
                        return true;
                    }
                    else
                    {
                        SqlCommand deleteCommand = new SqlCommand($"DELETE FROM [ProsisDTC].[dbo].[KeysInUse] WHERE [Key] = '{key}'" +
                            $"DELETE FROM", sql);
                        deleteCommand.ExecuteNonQuery();
                        sql.Close();
                        return false;
                    }
                }
                catch (SqlException ex)
                {
                    sql.Close();
                    _apiLogger.WriteLog(ex, "DeleteUser");
                    return false;
                }
            }
        }
    }
}
