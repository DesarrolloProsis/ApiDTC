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
        public Response GetStoredDtcData(DtcData dtcData)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand($"insert into DTCData (ReferenceNumber, SinisterNumber, ReportNumber, SinisterDate, FailureDate, FailureNumber, ShippingDate, ElaborationDate, Observation, Diagnosis, TypeDescriptionId, UserId, AgremmentInfoId)  values ('{dtcData.ReferenceNumber}','{dtcData.SinisterNumber}', '{dtcData.ReportNumber}', '{dtcData.SinisterDate.ToString("yyyy-MM-dd")}', '{dtcData.FailureDate.ToString("yyyy-MM-dd")}', '{dtcData.FailureNumber}', '{dtcData.ShippingDate.ToString("yyyy-MM-dd")}', '{dtcData.ElaborationDate.ToString("yyyy-MM-dd")}', '{dtcData.Observation}', '{dtcData.Diagnosis}', {dtcData.TypeDescriptionId}, {dtcData.UserId},  {dtcData.AgremmentInfoId} )", sql);
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
                        SqlCommand lastReferenceCommand = new SqlCommand($"SELECT TOP 1 ReferenceNumber FROM [ProsisDTC3].[dbo].[DTCData] WHERE ReferenceNumber LIKE '{referenceNumber}%' ORDER BY ReferenceNumber DESC", sql);
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

        private DtcData MapTodtcData(SqlDataReader reader)
        {
            return new DtcData()
            {
                ReferenceNumber = reader["ReferenceNumber"].ToString(),
                SinisterNumber = reader["SinisterNumber"].ToString(),
                ReportNumber = reader["ReportNumber"].ToString(),
                SinisterDate = Convert.ToDateTime(reader["SinisterDate"].ToString()),
                FailureDate = Convert.ToDateTime(reader["FailureDate"].ToString()),
                FailureNumber = reader["FailureNumber"].ToString(),
                ShippingDate = Convert.ToDateTime(reader["ShippingDate"].ToString()),
                ElaborationDate = Convert.ToDateTime(reader["ElaborationDate"].ToString()),
                Observation = reader["Observation"].ToString(),
                Diagnosis = reader["Diagnosis"].ToString(),
                TypeDescriptionId = (int)reader["TypeDescriptionId"],
                UserId = (int)reader["UserId"],
                AgremmentInfoId = (int)reader["AgremmentInfoId"],
                DateStamp = Convert.ToDateTime(reader["DateStamp"].ToString())
            };
        }
        private InvalidReferenceNumbers MapToInvalidReferenceNumbers(SqlDataReader reader)
        {
            return new InvalidReferenceNumbers()
            {
                SinisterNumber = reader["SinisterNumber"].ToString(),
                ReportNumber = reader["ReportNumber"].ToString(),
            };
        }
        #endregion
    }
}
