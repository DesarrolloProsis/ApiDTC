namespace ApiDTC.Data
{
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Microsoft.Extensions.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    public class SquaresCatalogDb
    {
        #region Attributes
        private readonly string _connectionString;

        private readonly SqlResult _sqlResult;

        private readonly ApiLogger _apiLogger;
        #endregion
        
        #region Constructor
        public SquaresCatalogDb(IConfiguration configuration, SqlResult sqlResult, ApiLogger apiLogger)
        {
            _apiLogger = apiLogger;
            _sqlResult = sqlResult;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods
        public Response GetSquaresCatalog(string clavePlaza)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("Select * From SquaresCatalog", sql);
                    return _sqlResult.GetList<SquaresCatalog>(clavePlaza, cmd, sql, "GetSquaresCatalog");
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "SquaresCatalogDb: GetSquaresCatalog", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetLanes(string square)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spSquareLanes", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Square", SqlDbType.NVarChar).Value = square;

                        var storedResult = _sqlResult.GetList<Lanes>("USR", cmd, sql, "GetLanes");
                        if (storedResult.Result == null)
                            return storedResult;

                        return new Response
                        {
                            Message = "Ok",
                            Result = storedResult.Result
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("USR", ex, "SquaresCatalog: GetLanes", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        #endregion
    }
}
