namespace ApiDTC.Data
{
    using System;
    using System.Collections.Generic;
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
                    using (SqlCommand cmd = new SqlCommand("dbo.spInsertFualtDiagnosisLanes", sql))
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

        public Response GetDiagnosticos(string clavePlaza, int userId, string _disk, string _folder)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[dbo].[spGetDiagnosisSheetView]", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Iduser", SqlDbType.Int).Value = userId;
                        var listaSheetView = _sqlResult.GetRows<DiagnosisSheetView>(clavePlaza, cmd, sql, "GetDiagnosticos");
                        
                        List<DiagnosisSheetViewValid> listDiagnosis = new List<DiagnosisSheetViewValid>();

                        foreach(var objDiagnosis in listaSheetView){

                            // listDiagnosis.Add(new DiagnosisSheetViewValid{                   
                            //     DiagnosticoSellado = false,
                            //     FichaSellado = false                                
                            // });  
                            DiagnosisSheetViewValid newObjDiagnosis = new DiagnosisSheetViewValid();

                            newObjDiagnosis.ReferenceNumber = objDiagnosis.ReferenceNumber;
                            newObjDiagnosis.SquareName = objDiagnosis.SquareName;
                            newObjDiagnosis.DiagnosisDate = objDiagnosis.DiagnosisDate;
                            newObjDiagnosis.Lanes = objDiagnosis.Lanes;
                            newObjDiagnosis.FailuerNumber = objDiagnosis.FailuerNumber;
                            newObjDiagnosis.SiniesterNumber = objDiagnosis.SiniesterNumber;
                            newObjDiagnosis.ValidacionFichaTecnica = objDiagnosis.ValidacionDTC;
                            newObjDiagnosis.SquareId = objDiagnosis.SquareId;
                            newObjDiagnosis.CauseFailure = objDiagnosis.CauseFailure;
                            newObjDiagnosis.FailureDescription = objDiagnosis.CauseFailure;
                            newObjDiagnosis.FailureDiagnosis = objDiagnosis.FailureDiagnosis;
                            newObjDiagnosis.Start = objDiagnosis.Start;
                            newObjDiagnosis.End = objDiagnosis.End;
                            newObjDiagnosis.AdminSquareId = objDiagnosis.AdminSquareId;
                            newObjDiagnosis.Intervention = objDiagnosis.Intervention;
                            newObjDiagnosis.FaultDescription = objDiagnosis.FaultDescription;
                            newObjDiagnosis.TypeFaultId = objDiagnosis.TypeFaultId;
                            newObjDiagnosis.ValidacionDTC = objDiagnosis.ValidacionDTC;
                            newObjDiagnosis.ReferenceDTC = objDiagnosis.ReferenceDTC;
                            string ruta = $@"{_disk}:/{_folder}/{objDiagnosis.ReferenceNumber.Split('-')[0]}/Reportes/{objDiagnosis.ReferenceNumber}/{objDiagnosis.ReferenceNumber}";
                            
                            if(System.IO.File.Exists(ruta + "-DiagnosticoSellado.pdf")){
                                newObjDiagnosis.DiagnosticoSellado = true;
                            }
                            else{
                                newObjDiagnosis.DiagnosticoSellado = false;
                            }

                            if(System.IO.File.Exists(ruta + "-FichaTecnicaSellado.pdf")){
                                newObjDiagnosis.FichaSellado = true;
                            }
                            else{
                                newObjDiagnosis.FichaSellado = false;
                            }

                            listDiagnosis.Add(newObjDiagnosis);

                        }      
                        return new Response{
                            Message = "ListaShettViewValidada",
                            Result = listDiagnosis,
                            Rows = listDiagnosis.Count
                        };                 
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DiagnosticoDb: GetDiagnosticoInfo", 1);
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

        //FIXME: Agregar descripcion
        public Response GetReferenceNumberDiagnosis(string clavePlaza, string value)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[dbo].[spGetReferenceNumberDiagnosis]", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@strReference", SqlDbType.NVarChar).Value = value;
                        return _sqlResult.GetList<Reference>(clavePlaza, cmd, sql, "GetReferenceNumberDiagnosis");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DiagnosticoDb: GetReferenceNumberDiagnosis", 1);
                return null;
            }
        }

        //Borrado Full

        public Response BorraDiagnosticoFull(string clavePlaza, string ReferenceNumber, int UserId, string Comment, string ReferenceDTC)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spDeleteDiagnosisFull", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = ReferenceNumber;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = Comment;
                        var storedResult = _sqlResult.Post(clavePlaza, cmd, sql, "InsertFichaTecnicaIntervencionLane");
                        if (storedResult.SqlResult == null)
                            return new Response { Message = "Error: " + storedResult.SqlMessage, Result = null };
                    }
                }
                return new Response
                {
                    Message = "Ok",
                    Result = ""
                };
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "Diagnostico: BorraDiagnosticoFull", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        #endregion
    }
}