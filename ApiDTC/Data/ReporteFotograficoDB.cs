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
        #endregion

        #region Constructor
        public ReporteFotograficoDB(IConfiguration configuration, ApiLogger apiLogger, SqlResult sqlResult)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods
        public DataSet GetStorePDF(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
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
        #endregion
    }
}
