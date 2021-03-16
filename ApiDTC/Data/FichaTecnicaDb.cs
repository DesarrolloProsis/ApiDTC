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
        public Response InsertDiagnosticoFichaTecnica(string clavePlaza, FichaTecnicaDiagnostico fichaTecnicaDiagnostico)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spInsertFaultDiagnosis"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = fichaTecnicaDiagnostico.ReferenceNumber;
                        cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = fichaTecnicaDiagnostico.SquareId;
                        cmd.Parameters.Add("@CapufeLaneNum", SqlDbType.NVarChar).Value = fichaTecnicaDiagnostico.CapufeLaneNum;
                        cmd.Parameters.Add("@IdGare", SqlDbType.NVarChar).Value = fichaTecnicaDiagnostico.IdGare;
                        cmd.Parameters.Add("@DiagnosisDate", SqlDbType.Date).Value = fichaTecnicaDiagnostico.DiagnosisDate;
                        cmd.Parameters.Add("@Start", SqlDbType.NVarChar).Value = fichaTecnicaDiagnostico.Start;
                        cmd.Parameters.Add("@End", SqlDbType.NVarChar).Value = fichaTecnicaDiagnostico.End;
                        cmd.Parameters.Add("@FailureNumber", SqlDbType.NVarChar).Value = fichaTecnicaDiagnostico.FailureNumber;
                        cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = fichaTecnicaDiagnostico.UserId;
                        cmd.Parameters.Add("@FailureDescription", SqlDbType.NVarChar).Value = fichaTecnicaDiagnostico.FailureDescription;
                        cmd.Parameters.Add("@FailureDiagnosis", SqlDbType.NVarChar).Value = fichaTecnicaDiagnostico.FailureDiagnosis;
                        cmd.Parameters.Add("@CauseFailure", SqlDbType.NVarChar).Value = fichaTecnicaDiagnostico.CauseFailure;
                        cmd.Parameters.Add("@AdminSquareId", SqlDbType.Int).Value = fichaTecnicaDiagnostico.AdminSquareId;

                        var storedResult = _sqlResult.Post(clavePlaza, cmd, sql, "InsertDiagnosticoFichaTecnica");
                        if (storedResult.SqlResult == null)
                            return new Response { Message = "Error: " + storedResult.SqlMessage, Result = null };
                    }
                }
                return new Response
                {
                    Message = "Ok",
                    Result = fichaTecnicaDiagnostico
                };
            }
            catch(SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "FichaTecnicaDb: InsertDiagnosticoFichaTecnica", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response InsertFichaTecnicaIntervencionLane(string clavePlaza, FichaTecnicaIntervencionLane fichaTecnicaIntervencionLane)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spInsertFaultDiagnosis"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = fichaTecnicaIntervencionLane.ReferenceNumber;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@CapuLaneNum", SqlDbType.NVarChar).Value = fichaTecnicaIntervencionLane.CapuLaneNum;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@IdGare", SqlDbType.NVarChar).Value = fichaTecnicaIntervencionLane.IdGare;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@AddFlag", SqlDbType.Bit).Value = fichaTecnicaIntervencionLane.AddFlag;
                        
                        var storedResult = _sqlResult.Post(clavePlaza, cmd, sql, "InsertFichaTecnicaIntervencionLane");
                        if (storedResult.SqlResult == null)
                            return new Response { Message = "Error: " + storedResult.SqlMessage, Result = null };
                    }
                }
                return new Response
                {
                    Message = "Ok",
                    Result = fichaTecnicaIntervencionLane
                };
            }
            catch(SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "FichaTecnicaDb: InsertFichaTecnicaIntervencionLane", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        #endregion
    }
}