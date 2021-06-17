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
    public class ComentarioDb
    {
        #region Attributes
        private readonly string _connectionString;

        private readonly SqlResult _sqlResult;

        private readonly ApiLogger _apiLogger;
        #endregion

        public ComentarioDb(IConfiguration configuration, SqlResult sqlResult, ApiLogger apiLogger)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }

        public SqlResponse InsertComment(string clavePlaza, Comment comentario)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spInsertUsersComments", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = comentario.TextoComment;
                        cmd.Parameters.Add("@CommentId", SqlDbType.Int).Value = comentario.CommentId;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = comentario.UserId;
                        return _sqlResult.Post(clavePlaza, cmd, sql, "insertComment");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ComentarioDb: InsertComment", 1);
                return new SqlResponse { SqlMessage = ex.Message, SqlResult = null };
            }
        }
    }
}
