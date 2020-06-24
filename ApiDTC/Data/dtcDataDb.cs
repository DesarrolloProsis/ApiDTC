﻿namespace ApiDTC.Data
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

        private SqlResult _sqlResult;

        private ApiLogger _apiLogger;
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

        //TODO Insert generic method
        public SqlResponse GetStoredDtcData(DtcData dtcData)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_InsertDtcData", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@referenceNumber", SqlDbType.NVarChar).Value = dtcData.ReferenceNumber;
                    
                    cmd.Parameters.Add("@sinisterNumber", SqlDbType.NVarChar);
                    if(string.IsNullOrEmpty(dtcData.SinisterNumber))
                        cmd.Parameters["@sinisterNumber"].Value = DBNull.Value;
                    else
                        cmd.Parameters["@sinisterNumber"].Value = dtcData.SinisterNumber;

                    cmd.Parameters.Add("@reportNumber", SqlDbType.NVarChar);
                    if(string.IsNullOrEmpty(dtcData.ReportNumber))
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
                    cmd.Parameters.Add("@DTCStatus", SqlDbType.Int).Value = dtcData.DTCStatus;
                    return _sqlResult.Post(cmd, sql);
                }
            }
        }
        //TODO Count generic method
        public Response GetReferenceNumber(string referenceNumber)
        {
            using(SqlConnection sql = new SqlConnection(_connectionString))
            {
                try
                {
                    sql.Open();
                    if(sql.State != ConnectionState.Open)
                    {
                        return new Response
                        {
                            Message = "Sql connection is closed",
                            Result = null
                        };
                    }

                    SqlCommand countCommand = new SqlCommand($"SELECT Count(*) FROM [ProsisDTC].[dbo].[DTCData] WHERE ReferenceNumber LIKE '{referenceNumber}%'", sql);
                    Int32 count = (Int32) countCommand.ExecuteScalar();
                    if(count == 0)
                    {
                        return new Response
                        {
                            Message = "Ok",
                            Result = $"{referenceNumber}"
                        };
                    }
                    else if(count == 1)
                    {
                        return new Response
                        {
                            Message = "Ok",
                            Result = $"{referenceNumber}-02"
                        };
                    }
                    else
                    {
                        SqlCommand lastReferenceCommand = new SqlCommand($"SELECT TOP 1 ReferenceNumber FROM [ProsisDTC].[dbo].[DTCData] WHERE ReferenceNumber LIKE '{referenceNumber}%' ORDER BY ReferenceNumber DESC", sql);
                        var reader = lastReferenceCommand.ExecuteReader();
                        if(reader.Read())
                        {
                            string result = reader["ReferenceNumber"].ToString();
                            int lastReference = Convert.ToInt32(result.Substring(result.Length - 1)) + 1;
                            return new Response
                            {
                                Message = "Ok",
                                Result = $"{referenceNumber}-{lastReference.ToString("00")}"
                            };
                        }
                        return new Response
                        {
                            Message = "Empty result",
                            Result = null
                        };
                    }
                }
                catch (SqlException ex)
                {
                    _apiLogger.WriteLog(ex, "GetStoredDtcData");
                    return new Response
                    {
                        Message = $"Error: {ex.Message}",
                        Result = null
                    };
                }
            }
        }
        public Response GetInvalidNumbers()
        {
            using(SqlConnection sql = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand($"SELECT SinisterNumber, ReportNumber FROM [ProsisDTC].[dbo].[DTCData]", sql);
                return _sqlResult.GetList<InvalidReferenceNumbers>(cmd, sql);
            }
        }
        public Response GetDTC(int idUser, string squareCatalog)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("", sql);
                    cmd.CommandText = "select " +
                                        "ReferenceNumber, " +
	                                    "SinisterNumber, " +                                        
	                                    "ReportNumber, " +
	                                    "SinisterDate, " +
                                        "d.StatusId, " +
                                        "FailureDate, " + 
	                                    "FailureNumber, " + 
	                                    "ShippingDate, " +
	                                    "ElaborationDate, " +
                                        "DateStamp, " +
                                        "d.TypeDescriptionId, " +
                                        "t.Description as TypeDescription, " +                                                                                
                                        "Observation, " +
                                        "Diagnosis, " +
                                        "s.StatusDescription "+
                                      "from DTCData d " +
                                      "inner join UserSquare u " +
                                      "on d.UserId = u.UserId " +
                                      "inner join TypeDescriptions t on d.TypeDescriptionId = t.TypeDescriptionId " +
                                      "join DTCStatusCatalog s on d.StatusId = s.StatusId "+
                                      "where d.UserId = '" + idUser + "' and u.SquareCatalogId = '" + squareCatalog + "' and d.StatusId != 0 ";

                    var info_dtc = _sqlResult.GetList<DtcDataStr>(cmd, sql);                    
                    return info_dtc;
//
                }
                catch(SqlException ex)
                {
                    _apiLogger.WriteLog(ex, "GetStoredDtcData");
                    return new Response
                    {
                        Message = $"Error: {ex.Message}",
                        Result = null
                    };
                } 
            }
        }
        public Response GetTableForm(string refNum)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_DescProposedComponent", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = refNum;


                    var storedResult = _sqlResult.GetList<ComponentTableForm>(cmd, sql);
                    if (storedResult.Result == null)
                        return storedResult;
                    var list = (List<ComponentTableForm>)storedResult.Result;


                    return new Response
                    {
                        Message = "Ok",
                        Result = storedResult.Result
                    };
                }
            }
        }

        public SqlResponse DeleteDtcData(string referenceNumber)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_DeleteDTC", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@referenceNumber", SqlDbType.NVarChar).Value = referenceNumber;
                    return _sqlResult.Post(cmd, sql);
                }
            }
        }

        /*public SqlResponse EditReference(string referenceNumber)
        {
            using(SqlConnection sql = new SqlConnection(_connectionString))
            {
                using()
            }
        }*/

        #endregion
    }
}
