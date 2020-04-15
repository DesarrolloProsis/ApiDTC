namespace ApiDTC.Data
{
    using Models;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Data;
    using ApiDTC.Services;

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
        public InsertResponse GetStoredDtcData(DtcData dtcData)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
<<<<<<< HEAD
                try
                {
                    SqlCommand cmd = new SqlCommand($"insert into DTCData (ReferenceNumber, SinisterNumber, ReportNumber, SinisterDate, FailureDate, FailureNumber, ShippingDate, ElaborationDate, Observation, Diagnosis, TypeDescriptionId, UserId, AgremmentInfoId, DateStamp)  values ('{dtcData.ReferenceNumber}','{dtcData.SinisterNumber}', '{dtcData.ReportNumber}', '{dtcData.SinisterDate.ToString("yyyy-MM-dd")}', '{dtcData.FailureDate.ToString("yyyy-MM-dd")}', '{dtcData.FailureNumber}', '{dtcData.ShippingDate.ToString("yyyy-MM-dd")}', '{dtcData.ElaborationDate.ToString("yyyy-MM-dd")}', '{dtcData.Observation}', '{dtcData.Diagnosis}', {dtcData.TypeDescriptionId}, {dtcData.UserId},  {dtcData.AgremmentInfoId}, '{DateTime.Now.ToString("yyyy-MM-dd")}')", sql);
                    sql.Open();
                    bool insertUp = Convert.ToBoolean(cmd.ExecuteNonQuery());
                    sql.Close();
                    return new Response
                    {
                        Message = "Ok",
                        Result = $"{dtcData.ReferenceNumber}"
                    };               
                }
                catch (SqlException ex)
=======
                using(SqlCommand cmd = new SqlCommand("dbo.sp_InsertDtcData", sql))
>>>>>>> 8d96d5597f1a9207494859e40cea9ef7a082d5f0
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@referenceNumber", SqlDbType.NVarChar).Value = dtcData.ReferenceNumber  ;
                    cmd.Parameters.Add("@sinisterNumber", SqlDbType.NVarChar).Value = dtcData.SinisterNumber;
                    cmd.Parameters.Add("@reportNumber", SqlDbType.NVarChar).Value = dtcData.ReportNumber;
                    cmd.Parameters.Add("@sinisterDate", SqlDbType.Date).Value = dtcData.SinisterDate;
                    cmd.Parameters.Add("@failureDate", SqlDbType.Date).Value = dtcData.FailureDate;
                    cmd.Parameters.Add("@failureNumber", SqlDbType.NVarChar).Value = dtcData.FailureNumber;
                    cmd.Parameters.Add("@shippingDate", SqlDbType.Date).Value = dtcData.ShippingDate;
                    cmd.Parameters.Add("@elaborationDate", SqlDbType.Date).Value = dtcData.ElaborationDate;
                    cmd.Parameters.Add("@observation", SqlDbType.NVarChar).Value = dtcData.Observation;
                    cmd.Parameters.Add("@diagnosis", SqlDbType.NVarChar).Value = dtcData.Diagnosis;
                    cmd.Parameters.Add("@typeDescriptionId", SqlDbType.Int).Value = dtcData.TypeDescriptionId;
                    cmd.Parameters.Add("@userId", SqlDbType.Int).Value = dtcData.AgremmentInfoId;
                    cmd.Parameters.Add("@agremmentInfoId", SqlDbType.Int).Value = dtcData.AgremmentInfoId;
                    cmd.Parameters.Add("@dateStamp", SqlDbType.Date).Value = dtcData.DateStamp;

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

                    SqlCommand countCommand = new SqlCommand($"SELECT Count(*) FROM [ProsisDTC3].[dbo].[DTCData] WHERE ReferenceNumber LIKE '{referenceNumber}%'", sql);
                    Int32 count = (Int32) countCommand.ExecuteScalar();
                    if(count == 0)
                    {
                        return new Response
                        {
                            Message = "Ok",
                            Result = $"{referenceNumber.Substring(0, 9)}"
                        };
                    }
                    else if(count == 1)
                    {
                        return new Response
                        {
                            Message = "Ok",
                            Result = $"{referenceNumber.Substring(0, 9)}-02"
                        };
                    }
                    else
                    {
                        SqlCommand lastReferenceCommand = new SqlCommand($"SELECT TOP 1 ReferenceNumber FROM [ProsisDTC3].[dbo].[DTCData] WHERE ReferenceNumber LIKE '{referenceNumber.Substring(0, 9)}%' ORDER BY ReferenceNumber DESC", sql);
                        var reader = lastReferenceCommand.ExecuteReader();
                        if(reader.Read())
                        {
                            string result = reader["ReferenceNumber"].ToString();
                            int lastReference = Convert.ToInt32(result.Substring(result.Length - 1)) + 1;
                            return new Response
                            {
                                Message = "Ok",
                                Result = $"{referenceNumber.Substring(0, 9)}-{lastReference.ToString("00")}"
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
                SqlCommand cmd = new SqlCommand($"SELECT SinisterNumber, ReportNumber FROM [ProsisDTC3].[dbo].[DTCData]", sql);
                return _sqlResult.GetList<InvalidReferenceNumbers>(cmd, sql);
            }
        }

        public Response GetDTC()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("Select * From DTCData", sql);
                    return _sqlResult.GetList<DtcData>(cmd, sql);
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
        #endregion
    }
}
