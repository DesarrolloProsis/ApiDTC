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

        private ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public DtcDataDb(IConfiguration configuration, ApiLogger apiLogger)
        {
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods
        public object GetStoredDtcData(DtcData dtcData)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("",sql))
                {
                    try
                    {
                        string query = string.Empty;

                        //Query para saber si existe ReferenceNumber
                        query = $"insert into DTCData (ReferenceNumber, SinisterNumber, ReportNumber, SinisterDate, FailureDate, FailureNumber, ShippingDate, ElaborationDate, Observation, Diagnosis, TypeDescriptionId, UserId, AgremmentInfoId,DateStamp)  values ('{dtcData.ReferenceNumber}','{dtcData.SinisterNumber}', '{dtcData.ReportNumber}', '{dtcData.SinisterDate.ToString("yyyy-MM-dd")}', '{dtcData.FailureDate.ToString("yyyy-MM-dd")}', '{dtcData.FailureNumber}', '{dtcData.ShippingDate.ToString("yyyy-MM-dd")}', '{dtcData.ElaborationDate.ToString("yyyy-MM-dd")}', '{dtcData.Observation}', '{dtcData.Diagnosis}', {dtcData.TypeDescriptionId}, {dtcData.UserId},  {dtcData.AgremmentInfoId}, '{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}' )";
                        sql.Open();
                        cmd.CommandText = query;
                        bool insertUp = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        return insertUp;
                  
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\nMessage ---\n{0}", ex.Message);
                        return ex.Message;
                    }
                    finally
                    {
                        sql.Close();
                    }
                }
            }
        }

        public SqlResult GetInvalidNumbers()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                try
                {
                    sql.Open();
                    if (sql.State != ConnectionState.Open)
                    {
                        return new SqlResult
                        {
                            Message = "Sql connection is closed",
                            Result = null
                        };
                    }

                    SqlCommand cmd = new SqlCommand($"SELECT SinisterNumber, ReportNumber FROM [ProsisDTC3].[dbo].[DTCData]", sql);
                    var reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        return new SqlResult
                        {
                            Message = "Result not found",
                            Result = null
                        };
                    }
                    var response = new List<InvalidReferenceNumbers>();
                    while (reader.Read())
                    {
                        response.Add(MapToInvalidReferenceNumbers(reader));
                    }
                    sql.Close();
                    return new SqlResult
                    {
                        Message = "Ok",
                        Result = response
                    };
                }
                catch (SqlException ex)
                {
                    _apiLogger.WriteLog(ex, "GetValidNumbers");
                    return new SqlResult
                    {
                        Message = $"Error: {ex.Message}",
                        Result = null
                    };
                }
            }
        }

        public SqlResult GetReferenceNumber(string referenceNumber)
        {
            using(SqlConnection sql = new SqlConnection(_connectionString))
            {
                try
                {
                    sql.Open();
                    if(sql.State != ConnectionState.Open)
                    {
                        return new SqlResult
                        {
                            Message = "Sql connection is closed",
                            Result = null
                        };
                    }

                    SqlCommand countCommand = new SqlCommand($"SELECT Count(*) FROM [ProsisDTC3].[dbo].[DTCData] WHERE ReferenceNumber LIKE '{referenceNumber}%'", sql);
                    Int32 count = (Int32) countCommand.ExecuteScalar();
                    if(count == 0)
                    {
                        return new SqlResult
                        {
                            Message = "Ok",
                            Result = $"{referenceNumber}"
                        };
                    }
                    else if (count == 1)
                    {
                        return new SqlResult
                        {
                            Message = "Ok",
                            Result = $"{referenceNumber}-02"
                        };
                    }
                    else
                    {
                        SqlCommand lastReferenceCommand = new SqlCommand($"SELECT TOP 1 ReferenceNumber FROM [ProsisDTC3].[dbo].[DTCData] WHERE ReferenceNumber LIKE '{referenceNumber}%' ORDER BY ReferenceNumber DESC", sql);
                        using (SqlDataReader reader = lastReferenceCommand.ExecuteReader())
                        {
                            if(reader.HasRows)
                            {
                                if(reader.Read())
                                {
                                    string result = reader["ReferenceNumber"].ToString();
                                    int lastReference = Convert.ToInt32(result.Substring(result.Length - 1)) + 1;
                                    return new SqlResult
                                    {
                                        Message = "Ok",
                                        Result = $"{referenceNumber}-{lastReference.ToString("00")}"
                                    };
                                }
                            }
                        }
                    }
                    return new SqlResult
                    {
                        Message = $"Error: ",
                        Result = null
                    };
                }
                catch(SqlException ex)
                {
                    return new SqlResult
                    {
                        Message = $"Error: {ex.Message}",
                        Result = null
                    };
                }
            }
        }
        public string GetReferenceNum(string refNum)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {

                using (SqlCommand cmd = new SqlCommand("", sql))
                {
                    try
                    {
                        string query = string.Empty;
                        //Query para saber si existe ReferenceNumber
                        query = $"Select Count(*) From DTCData  where ReferenceNumber = '{refNum}' ";
                        sql.Open();
                        cmd.CommandText = query;
                        bool exist = Convert.ToBoolean(cmd.ExecuteScalar());
                        if (exist)
                        {
                            query = $"Select Count(*) From DTCData  where ReferenceNumber = '{refNum}-02' ";
                            cmd.CommandText = query;
                            exist = Convert.ToBoolean(cmd.ExecuteScalar());
                            if (exist)
                            {
                                query = $"Select Count(*) From DTCData  where ReferenceNumber = '{refNum}-03' ";
                                cmd.CommandText = query;
                                exist = Convert.ToBoolean(cmd.ExecuteScalar());
                                if (exist)
                                {
                                    query = $"Select Count(*) From DTCData  where ReferenceNumber = '{refNum}-04' ";
                                    cmd.CommandText = query;
                                    exist = Convert.ToBoolean(cmd.ExecuteScalar());
                                    if (exist)
                                    {
                                        query = $"Select Count(*) From DTCData  where ReferenceNumber = '{refNum}-05' ";
                                        cmd.CommandText = query;
                                        exist = Convert.ToBoolean(cmd.ExecuteScalar());
                                        var refNum2 = refNum + "-05";
                                        return refNum2;
                                    }
                                    else
                                    {
                                        var refNum2 = refNum + "-04";
                                        return refNum2;
                                    }
                                }
                                else
                                {
                                    var refNum2 = refNum + "-03";
                                    return refNum2;
                                }
                            }
                            else
                            {
                                var refNum2 = refNum + "-02";
                                return refNum2;
                            }
                        }
                        else
                        {
                            var response = new List<DtcData>();
                            return refNum;
                        }
                        //var response = new List<DTCData>();
                        //int noRegistros = Convert.ToInt32(cmd.ExecuteScalar());
                        //return noRegistros;

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\nMessage ---\n{0}", ex.Message);
                        return null;
                    }
                    finally
                    {
                        sql.Close();
                    }
                }
            }
        }

        public List<DtcData> GetDTC()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {

                using (SqlCommand cmd = new SqlCommand("", sql))
                {
                    try
                    {

                        string query = string.Empty;

                        //Query para saber si existe ReferenceNumber
                        query = $"Select * From DTCData";
                        sql.Open();
                        cmd.CommandText = query;
                        var response = new List<DtcData>();
                        var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            response.Add(MapTodtcData(reader));
                        }

                        return response;
                        //int noRegistros = Convert.ToInt32(cmd.ExecuteScalar());

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\nMessage ---\n{0}", ex.Message);
                        return null;
                    }
                    finally
                    {
                        sql.Close();
                    }
                }
            }
        }

        private InvalidReferenceNumbers MapToInvalidReferenceNumbers(SqlDataReader reader)
        {
            return new InvalidReferenceNumbers()
            {
                SinisterNumber = reader["SinisterNumber"].ToString(),
                ReportNumber = reader["ReportNumber"].ToString(),
            };
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
            };
        }
        #endregion
    }
}
