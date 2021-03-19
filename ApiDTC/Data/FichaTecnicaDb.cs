namespace ApiDTC.Data
{
    using System.Data;
    using System.Data.SqlClient;
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Microsoft.Extensions.Configuration;

    public class FichaTecnicaDb
    {
        #region Attributes
        private readonly string _connectionString;

        private readonly SqlResult _sqlResult;

        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public FichaTecnicaDb(IConfiguration configuration, SqlResult sqlResult, ApiLogger apiLogger)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion
        
        #region Methods
            public Response InsertFichaTecnica(string clavePlaza, FichaTecnica fichaTecnica)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spInsertTechnicalSheet"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@TypeFaultId", SqlDbType.Int).Value = fichaTecnica.TypeFaultId;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = fichaTecnica.ReferenceNumber;
                        cmd.Parameters.Add("@Intervention", SqlDbType.NVarChar).Value = fichaTecnica.Intervention;
                        cmd.Parameters.Add("@UpdateFlag", SqlDbType.Bit).Value = fichaTecnica.UpdateFlag;

                        var storedResult = _sqlResult.Post(clavePlaza, cmd, sql, "InsertFichaTecnica");
                        if (storedResult.SqlResult == null)
                            return new Response { Message = "Error: " + storedResult.SqlMessage, Result = null };
                    }
                }
                return new Response
                {
                    Message = "Ok",
                    Result = fichaTecnica
                };
            }
            catch(SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "FichaTecnicaDb: InsertFichaTecnica", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetTechnicalSheet(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[dbo].[spGetTechnicalSheet]", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;
                        var storedResult =  _sqlResult.GetList<FichaTecnicaInfo>(clavePlaza, cmd, sql, "GetTechnicalSheet");
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
                _apiLogger.WriteLog(clavePlaza, ex, "FichaTecnicaDb: GetTechnicalSheet", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public FichaTecnicaInfo GetFichaTecnicaPdf(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[dbo].[spGetTechnicalSheet]", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;
                        return _sqlResult.GetRow<FichaTecnicaInfo>(clavePlaza, cmd, sql, "GetFichaTecnicaPdf");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "FichaTecnicaDb: GetFichaTecnicaPdf", 1);
                return null;
            }
        }
        #endregion
    }
}