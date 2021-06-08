﻿namespace ApiDTC.Data
{
    using Models;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Data.SqlClient;
    using System.Data;
    using ApiDTC.Services;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.IO;

    public class DtcDataDb
    {
        #region Attributes
        private readonly string _connectionString;

        private readonly SqlResult _sqlResult;

        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public DtcDataDb(IConfiguration configuration, SqlResult sqlResult, ApiLogger apiLogger)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods

        public SqlResponse GetStoredDtcData(string clavePlaza, DtcData dtcData)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_InsertDtcData", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@referenceNumber", SqlDbType.NVarChar).Value = dtcData.ReferenceNumber;

                        cmd.Parameters.Add("@sinisterNumber", SqlDbType.NVarChar);
                        if (string.IsNullOrEmpty(dtcData.SinisterNumber))
                            cmd.Parameters["@sinisterNumber"].Value = DBNull.Value;
                        else
                            cmd.Parameters["@sinisterNumber"].Value = dtcData.SinisterNumber;

                        cmd.Parameters.Add("@reportNumber", SqlDbType.NVarChar);
                        if (string.IsNullOrEmpty(dtcData.ReportNumber))
                            cmd.Parameters["@reportNumber"].Value = DBNull.Value;
                        else
                            cmd.Parameters["@reportNumber"].Value = dtcData.ReportNumber;

                        //SQuare
                        dtcData.SquareId = dtcData.SquareId == "1Bi" ? "1Bis" : dtcData.SquareId;

                        cmd.Parameters.Add("@sinisterDate", SqlDbType.Date).Value = dtcData.SinisterDate;
                        cmd.Parameters.Add("@failureDate", SqlDbType.Date).Value = dtcData.FailureDate;
                        cmd.Parameters.Add("@failureNumber", SqlDbType.NVarChar).Value = dtcData.FailureNumber;
                        cmd.Parameters.Add("@shippingDate", SqlDbType.Date).Value = dtcData.ShippingDate;
                        cmd.Parameters.Add("@elaborationDate", SqlDbType.Date).Value = dtcData.ElaborationDate;
                        cmd.Parameters.Add("@observation", SqlDbType.NVarChar).Value = dtcData.Observation;
                        cmd.Parameters.Add("@diagnosis", SqlDbType.NVarChar).Value = dtcData.Diagnosis;
                        cmd.Parameters.Add("@typeDescriptionId", SqlDbType.Int).Value = dtcData.TypeDescriptionId;
                        cmd.Parameters.Add("@userId", SqlDbType.Int).Value = dtcData.UserId;
                        cmd.Parameters.Add("@agremmentInfoId", SqlDbType.Int).Value = dtcData.AgremmentInfoId;
                        cmd.Parameters.Add("@status", SqlDbType.Int).Value = dtcData.DTCStatus;
                        cmd.Parameters.Add("@flag", SqlDbType.Bit).Value = dtcData.Flag;
                        cmd.Parameters.Add("@openFlag", SqlDbType.Bit).Value = dtcData.OpenFlag;
                        cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = dtcData.SquareId;
                        cmd.Parameters.Add("@AdminId", SqlDbType.Int).Value = dtcData.AdminId;
                        cmd.Parameters.Add("@DiagnosisReference", SqlDbType.NVarChar).Value = dtcData.DiagnosisReference;

                        return _sqlResult.Post(clavePlaza, cmd, sql, "GetStoredDtcData");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataDb: GetStoredDtcData", 1);
                return new SqlResponse { SqlMessage = ex.Message, SqlResult = null };
            }
        }

        public Response GetReferenceNumber(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spGetReferenceNumber", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@strReference", SqlDbType.NVarChar).Value = referenceNumber;
                        return _sqlResult.GetList<Reference>(clavePlaza, cmd, sql, "GetReferenceNumber");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataDb: GetReferenceNumber", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response UpdateDtcHeader(string clavePlaza, DtcHeader dtcHeader)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spUpdateDTCHeader", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = dtcHeader.ReferenceNumber;
                        cmd.Parameters.Add("@NumSiniestro", SqlDbType.NVarChar).Value = String.IsNullOrEmpty(dtcHeader.NumSiniestro) ? SqlString.Null : dtcHeader.NumSiniestro;
                        cmd.Parameters.Add("@NumReporte", SqlDbType.NVarChar).Value = String.IsNullOrEmpty(dtcHeader.NumReporte) ? SqlString.Null : dtcHeader.NumReporte;
                        cmd.Parameters.Add("@FolioFalla", SqlDbType.NVarChar).Value = String.IsNullOrEmpty(dtcHeader.FolioFalla) ? SqlString.Null : dtcHeader.FolioFalla;
                        cmd.Parameters.Add("@TipoDescripcion", SqlDbType.Int).Value = dtcHeader.TipoDescripcion;
                        cmd.Parameters.Add("@observaciones", SqlDbType.NVarChar).Value = String.IsNullOrEmpty(dtcHeader.Observaciones) ? SqlString.Null : dtcHeader.Observaciones;
                        cmd.Parameters.Add("@Diagnostico", SqlDbType.NVarChar).Value = String.IsNullOrEmpty(dtcHeader.Diagnostico) ? SqlString.Null : dtcHeader.Diagnostico;
                        var response = _sqlResult.Put(clavePlaza, cmd, sql, "UpdateDtcHeader");
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
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataDb: UpdateDtcHeader", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetReferencesLog()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spGetReferencesLog", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        return _sqlResult.GetList<ReferenceLog>("USR", cmd, sql, "GetReferencesLog");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("USR", ex, "DtcDataDb: GetReferencesLog", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetReferencesLogDetails(string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spGetReferencesLogDetails", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;
                        return _sqlResult.GetList<ReferenceLogDetail>("USR", cmd, sql, "spGetReferencesLogDetails");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog("USR", ex, "DtcDataDb: GetReferencesLogDetails", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetComponentsInventoryList(string clavePlaza, string squareId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("select b.Lane, a.Component, a.SerialNumber, cast(a.InstalationDate as nvarchar) InstallationDate, cast(a.MaintenanceDate as nvarchar) MaintenanceDate, MaintenanceFolio, TableFolio " +
                        "from SquareInventory a join LanesCatalog b on (a.CapufeLaneNum = b.CapufeLaneNum and a.IdGare = b.IdGare) " +
                        "join SquaresCatalog c on b.SquareCatalogId = c.SquareCatalogId " +
                        $"where c.SquareCatalogId = '{squareId}'", sql);
                    return _sqlResult.GetList<ComponentsInventoryList>(clavePlaza, cmd, sql, "GetComponentsInventoryList");
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataDb: GetComponentsInventoryList", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetInvalidNumbers(string clavePlaza)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand($"SELECT SinisterNumber, ReportNumber FROM [DTCData]", sql);
                    return _sqlResult.GetList<InvalidReferenceNumbers>(clavePlaza, cmd, sql, "GetInvalidNumbers");
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataDb: GetInvalidNumbers", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetDTC(string clavePlaza, int idUser, string squareCatalog, string disk, string folder)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spGetDTCView", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = idUser;
                        cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = squareCatalog;
                        var dtcViewList = _sqlResult.GetRows<DtcView>(clavePlaza, cmd, sql, "DtcDataDb: GetDTC");

                        List<DtcViewInfo> dtcViewInfo = new List<DtcViewInfo>();

                        foreach(var dtcView in dtcViewList)
                        {
                            DtcViewInfo viewInfo = new DtcViewInfo
                            {
                                DtcView = dtcView
                            };
                            string path = $@"{disk}:\{folder}\{dtcView.ReferenceNumber.Split('-')[0].ToUpper()}\DTC\{dtcView.ReferenceNumber}\DTC-{dtcView.ReferenceNumber}-Sellado.pdf";
                            if (System.IO.File.Exists((path)))
                                viewInfo.PdfExists = true;
                            else
                                viewInfo.PdfExists = false;

                            string directoy = $@"{disk}:\{folder}\{dtcView.ReferenceNumber.Split('-')[0].ToUpper()}\DTC\{dtcView.ReferenceNumber}\EquipoDañadoImgs";
                            List<string> dtcImages = new List<string>();
                            if(Directory.Exists(directoy))
                            {
                                if (Directory.GetFiles(directoy) != null)
                                {
                                foreach (var item in Directory.GetFiles(directoy))
                                    dtcImages.Add(item.Substring(item.LastIndexOf('\\') + 1));
                                }
                            }
                            
                            viewInfo.Paths = dtcImages;
                            
                            dtcViewInfo.Add(viewInfo);
                        }
                        return new Response
                        {
                            Result = dtcViewInfo,
                            Message = "Ok"
                        };
                        
                    }
                }
                catch (SqlException ex)
                {
                    _apiLogger.WriteLog(clavePlaza, ex, "DtcDataDb: GetDTC", 1);
                    return new Response { Message = $"Error: {ex.Message}", Result = null };
                }
            }
        }

        public Response GetTableForm(string clavePlaza, string refNum)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_DescProposedComponent", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = refNum;


                        var storedResult = _sqlResult.GetList<ComponentTableForm>(clavePlaza, cmd, sql, "GetTableForm");
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
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataDb: GetTableForm", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public SqlResponse DeleteDtcData(string clavePlaza, string referenceNumber, int userId, string Comment)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_DeleteDTC", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@referenceNumber", SqlDbType.NVarChar).Value = referenceNumber;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = Comment;
                        return _sqlResult.Post(clavePlaza, cmd, sql, "DeleteDtcData");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataDb: DeleteDtcData", 1);
                return new SqlResponse { SqlMessage = ex.Message, SqlResult = null };
            }
        }

        public Response EditReferece(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_EditReferencePrueba", sql))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();


                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;

                        sql.Open();
                        sqlDataAdapter = new SqlDataAdapter(cmd);
                        sqlDataAdapter.Fill(dataSet);

                        sql.Close();

                        if (dataSet.Tables[0].Rows.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                        {
                            return new Response
                            {
                                Message = "Data not found",
                                Result = null
                            };
                        }

                        var serialNumbers = _sqlResult.DataSetMapper<DtcItems>(clavePlaza, dataSet.Tables[0], "EditReferece");
                        if (serialNumbers == null)
                        {
                            return new Response
                            {
                                Message = "No se pudo obtener los números de serie. Revisar registros de error.",
                                Result = null
                            };
                        }
                        var items = _sqlResult.DataSetMapper<DtcSerialNumbers>(clavePlaza, dataSet.Tables[1], "EditReferece");
                        if (items == null)
                        {
                            return new Response
                            {
                                Message = "No se pudieron obtener los artículos. Revisar registros de error",
                                Result = null
                            };
                        }
                        return new Response
                        {
                            Message = "Ok",
                            Result = new EditReferenceInformation
                            {
                                SerialNumbers = items,
                                Items = serialNumbers
                            }
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataDb: EditReferece", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response EditRefereceOpen(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spEditReferenceOpen", sql))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();


                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;

                        sql.Open();
                        sqlDataAdapter = new SqlDataAdapter(cmd);
                        sqlDataAdapter.Fill(dataSet);

                        sql.Close();


                        if (dataSet.Tables[0].Rows.Count == 0 || dataSet.Tables[1].Rows.Count == 0)
                        {
                            return new Response
                            {
                                Message = "Data not found",
                                Result = null
                            };
                        }

                        var requestedComponent = _sqlResult.DataSetMapper<EditRequestedComponent>(clavePlaza, dataSet.Tables[0], "EditRefereceOpen");
                        if (requestedComponent == null)
                        {
                            return new Response
                            {
                                Message = "No se pudo obtener los números de serie. Revisar registros de error.",
                                Result = null
                            };
                        }

                        var proposedComponent = _sqlResult.DataSetMapper<ProposedComponentsOpen>(clavePlaza, dataSet.Tables[1], "EditRefereceOpen");
                        if (proposedComponent == null)
                        {
                            return new Response
                            {
                                Message = "No se pudieron obtener los artículos. Revisar registros de error",
                                Result = null
                            };
                        }

                        ResultEditReferenceOpen resultEditReferenceOpen = new ResultEditReferenceOpen
                        {
                            ProposedComponents = (List<ProposedComponentsOpen>)proposedComponent,
                            RequestedComponents = (List<EditRequestedComponent>)requestedComponent
                        };

                        return new Response
                        {
                            Message = "Ok",
                            Result = resultEditReferenceOpen
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataDb: EditRefereceOpen", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetDTCHeaderEdit(string clavePlaza, string ReferenceNumber)
        {

            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[dbo].[spGetHeaderEdit]", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = ReferenceNumber;
                        return _sqlResult.GetList<HeaderEditDTC>(clavePlaza, cmd, sql, "GetDTCHeaderEdit");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataDb: GetDTCHeaderEdit", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        #endregion
    }
}
