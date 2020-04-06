using ApiDTC.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Data
{
    public class DtcDataDb
    {
        private readonly string _connectionString;
        

        public DtcDataDb(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }


        public bool GetStoredtcData(DtcData dtcData)
        {

            
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {

                using (SqlCommand cmd = new SqlCommand("",sql))
                {

                    try
                    {
                        string query = string.Empty;

                        //Query para saber si existe ReferenceNumber
                        query = $"insert into DTCData (ReferenceNumber, SinisterNumber, ReportNumber, SinisterDate, FailureDate, FailureNumber, ShippingDate, ElaborationDate, Observation, Diagnosis, TypeDescriptionId, UserId, AgremmentInfoId)  values ('{dtcData.ReferenceNumber}','{dtcData.SinisterNumber}', '{dtcData.ReportNumber}', '{dtcData.SinisterDate.ToString("yyyy-MM-dd")}', '{dtcData.FailureDate.ToString("yyyy-MM-dd")}', '{dtcData.FailureNumber}', '{dtcData.ShippingDate.ToString("yyyy-MM-dd")}', '{dtcData.ElaborationDate.ToString("yyyy-MM-dd")}', '{dtcData.Observation}', '{dtcData.Diagnosis}', {dtcData.TypeDescriptionId}, {dtcData.UserId},  {dtcData.AgremmentInfoId} )";
                        sql.Open();
                        cmd.CommandText = query;
                        bool insertUp = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        return insertUp;
                  
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\nMessage ---\n{0}", ex.Message);
                        return false;
                    }
                    finally
                    {
                        sql.Close();
                    }
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

    }
}
