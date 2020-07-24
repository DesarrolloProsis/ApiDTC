namespace ApiDTC.Data
{
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Microsoft.Extensions.Configuration;
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
                    return _sqlResult.GetList<UserView>(cmd, sql);
                }
            }
        }
    }
}
