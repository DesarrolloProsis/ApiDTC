namespace ApiDTC.Data
{
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class ReporteFotograficoDB
    {
        #region Attributes

        private readonly string _connectionString;

        private readonly ApiLogger _apiLogger;

        private readonly SqlResult _sqlResult;

        #endregion Attributes

        #region Constructor

        public ReporteFotograficoDB(IConfiguration configuration, ApiLogger apiLogger, SqlResult sqlResult)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }

        #endregion Constructor

        #region Methods

        public DataSet GetStorePDF(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    //spPhotoReport DEBE TRAER EL DIAGNOSTICO
                    using (SqlCommand cmd = new SqlCommand("dbo.spPhotoReport", sql))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;

                        sql.Open();
                        sqlDataAdapter = new SqlDataAdapter(cmd);
                        sqlDataAdapter.Fill(dataSet);

                        sql.Close();

                        return dataSet;
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoDb: GetStorePDF", 1);
                return null;
            }
        }

        public DataSet GetStoreNuevoPDF(string clavePlaza, string referenceNumber, string referenceAnexo, bool isSubAnexo)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    //spPhotoReport DEBE TRAER EL DIAGNOSTICO
                    using (SqlCommand cmd = new SqlCommand("dbo.spPhotoReportNuevos", sql))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;
                        cmd.Parameters.Add("@ReferenceNumberAnexo", SqlDbType.NVarChar).Value = referenceAnexo;
                        cmd.Parameters.Add("@isSubVersion", SqlDbType.Bit).Value = isSubAnexo;

                        sql.Open();
                        sqlDataAdapter = new SqlDataAdapter(cmd);
                        sqlDataAdapter.Fill(dataSet);

                        sql.Close();

                        return dataSet;
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoDb: GetStorePDF", 1);
                return null;
            }
        }

        public DataSet GetStorePDFReporteFotografico(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spCalendarReportToPDF", sql))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;

                        sql.Open();
                        sqlDataAdapter = new SqlDataAdapter(cmd);
                        sqlDataAdapter.Fill(dataSet);

                        sql.Close();

                        return dataSet;
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoDb: GetStorePDF", 1);
                return null;
            }
        }

        public DataSet GetMostRecentAnexoReference(string clavePlaza, string referenceAnexo, bool isSubAnexo)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spMostRecentSubAnexo", sql))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumberAnexo", SqlDbType.NVarChar).Value = referenceAnexo;
                        cmd.Parameters.Add("@isSubVersion", SqlDbType.Bit).Value = isSubAnexo;

                        sql.Open();
                        sqlDataAdapter = new SqlDataAdapter(cmd);
                        sqlDataAdapter.Fill(dataSet);

                        sql.Close();

                        return dataSet;
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoDb: spMostRecentSubAnexo", 1);
                return null;
            }
        }

        #endregion Methods
    }
}