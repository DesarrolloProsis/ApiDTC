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

        public Response UpdateFolioFechaInventario(string clavePlaza, string IdGare, string NCapufe, string Fecha, string Folio, int IdUsuario)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spUpdateInventoryMonthly", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@IdGare", SqlDbType.NVarChar).Value = IdGare;
                        cmd.Parameters.Add("@NCapufe", SqlDbType.NVarChar).Value = NCapufe;
                        cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = Fecha;
                        cmd.Parameters.Add("@Folio", SqlDbType.NVarChar).Value = Folio;
                        cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = IdUsuario;
                        var response = _sqlResult.Put(clavePlaza, cmd, sql, "UpdateFolioFechaInventario");
                        return new Response
                        {
                            Message = response.SqlMessage,
                            Result = response.SqlResult
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "MantenimientoDb: UpdateFolioFechaInventario", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }


        #endregion
    }
}