namespace ApiDTC.Data
{
    using System.Data;
    using System.Data.SqlClient;
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Microsoft.Extensions.Configuration;

    public class DiagnosticoDb
    {
        #region Attributes
        private readonly string _connectionString;

        private readonly SqlResult _sqlResult;

        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public DiagnosticoDb(IConfiguration configuration, SqlResult sqlResult, ApiLogger apiLogger)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion
        
        #region Methods
        public Response InsertFaultDiagnosis(string clavePlaza, DiagnosticoDeFalla diagnosticoDeFalla)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString)) 
                { 
                    using (SqlCommand cmd = new SqlCommand("dbo.spInsertFaultDiagnosis", sql))
                    { 
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = diagnosticoDeFalla.ReferenceNumber;
                        cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = diagnosticoDeFalla.SquareId == "1Bi" ? diagnosticoDeFalla.SquareId + "s" : diagnosticoDeFalla.SquareId;
                        cmd.Parameters.Add("@DiagnosisDate", SqlDbType.Date).Value = diagnosticoDeFalla.DiagnosisDate;
                        cmd.Parameters.Add("@Start", SqlDbType.NVarChar).Value = diagnosticoDeFalla.Start;
                        cmd.Parameters.Add("@End", SqlDbType.NVarChar).Value = diagnosticoDeFalla.End;
                        cmd.Parameters.Add("@SinisterNumber", SqlDbType.NVarChar).Value = diagnosticoDeFalla.SinisterNumber;
                        cmd.Parameters.Add("@FailureNumber", SqlDbType.NVarChar).Value = diagnosticoDeFalla.FailureNumber;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = diagnosticoDeFalla.UserId;
                        cmd.Parameters.Add("@FailureDescription", SqlDbType.NVarChar).Value = diagnosticoDeFalla.FailureDescription;
                        cmd.Parameters.Add("@FailureDiagnosis", SqlDbType.NVarChar).Value = diagnosticoDeFalla.FailureDiagnosis;
                        cmd.Parameters.Add("@CauseFailure", SqlDbType.Int).Value = diagnosticoDeFalla.CauseFailure;
                        cmd.Parameters.Add("@AdminSquareId", SqlDbType.Int).Value = diagnosticoDeFalla.AdminSquareId;
                        cmd.Parameters.Add("@UpdateFlag", SqlDbType.Bit).Value = diagnosticoDeFalla.UpdateFlag;

                        var storedResult = _sqlResult.Post(clavePlaza, cmd, sql, "InsertFaultDiagnosis");
                        if (storedResult.SqlResult == null)
                            return new Response { Message = "No se pudo diagnóstico de falla " + diagnosticoDeFalla.ReferenceNumber, Result = null };
                    }
                }
                return new Response
                {
                    Message = "Ok",
                    Result = diagnosticoDeFalla
                };
            }
            catch(SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DiagnosticoDb: InsertFaultDiagnosis", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
            
        }
        #endregion
    }
}