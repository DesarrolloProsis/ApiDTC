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
        
        //Información del diagnóstico de falla
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
                        cmd.Parameters.Add("@CauseFailure", SqlDbType.NVarChar).Value = diagnosticoDeFalla.CauseFailure;
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

        //Carriles del diagnóstico de falla
        public Response InsertFichaTecnicaIntervencionLane(string clavePlaza, FichaTecnicaIntervencionLane fichaTecnicaIntervencionLane)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spInsertFualtDiagnosisLanes"))
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

        //Información diagnóstico de falla formato Response
        public Response GetDiagnosticoInfo(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[dbo].[spGetFaultDiagnosis]", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;
                        return _sqlResult.GetList<DiagnosticoDeFallaInfo>(clavePlaza, cmd, sql, "GetDiagnosticoInfo");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DiagnosticoDb: GetDiagnosticoInfo", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        //Información diagnóstico de falla formato PDF
        public DiagnosticoDeFallaInfo GetDiagnosticoInfoPdf(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[dbo].[spGetFaultDiagnosis]", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;
                        return _sqlResult.GetRow<DiagnosticoDeFallaInfo>(clavePlaza, cmd, sql, "GetDiagnosticoInfoPdf");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DiagnosticoDb: GetDiagnosticoInfo", 1);
                return null;
            }
        }
        #endregion
    }
}