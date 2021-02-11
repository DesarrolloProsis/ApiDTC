namespace ApiDTC.Data
{
    using System.Data;
    using ApiDTC.Models;
    using System.Data.SqlClient;
    using ApiDTC.Services;
    using Microsoft.Extensions.Configuration;

    public class MantenimientoDb
    {
        #region Attributes
        private readonly string _connectionString;

        private readonly ApiLogger _apiLogger;

        private readonly SqlResult _sqlResult;
        #endregion

        #region Constructor
        public MantenimientoDb(IConfiguration configuration, ApiLogger apiLogger, SqlResult sqlResult)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods
        public Response GetBitacora()
        {
            try
            {
                using(SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using(SqlCommand cmd = new SqlCommand("dbo.spBitacoraMantenimientoPreventivo", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var storedResult = _sqlResult.GetList<Bitacora>("GMP", cmd, sql, "GetBitacora");
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
                _apiLogger.WriteLog("GMP", ex, "MantenimientoDb: GetBitacora", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        #endregion
    }
}