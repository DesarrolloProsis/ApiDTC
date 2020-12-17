namespace ApiDTC.Data
{
    using Models;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Data.SqlClient;
    using System.Data;
    using ApiDTC.Services;
    using System.Collections.Generic;

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

                        cmd.Parameters.Add("@sinisterDate", SqlDbType.Date).Value = dtcData.SinisterDate;
                        cmd.Parameters.Add("@failureDate", SqlDbType.Date).Value = dtcData.FailureDate;
                        cmd.Parameters.Add("@failureNumber", SqlDbType.NVarChar).Value = dtcData.FailureNumber.PadLeft(6, '0');
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
                using(SqlConnection sql = new SqlConnection(_connectionString))
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

        public Response UpdateDtcStatus(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spUpdateStatusDTC", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;
                        var response = _sqlResult.Put(clavePlaza, cmd, sql, "UpdateDtcStatus");
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
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataDb: UpdateDtcStatus", 1);
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

        //TEST
        public Response GetDTC(string clavePlaza, int idUser, string squareCatalog)
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
                        return _sqlResult.GetList<DtcView>(clavePlaza, cmd, sql, "GetDTC");
                    }
                }
                catch(SqlException ex)
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

        public SqlResponse DeleteDtcData(string clavePlaza, string referenceNumber, int userId)
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
