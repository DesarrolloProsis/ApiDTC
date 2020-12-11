namespace ApiDTC.Data
{
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class PdfConsultasDb
    {
        #region Attributes
        private readonly string _connectionString;

        private ApiLogger _apiLogger;

        private SqlResult _sqlResult;
        #endregion

        #region Constructor
        public PdfConsultasDb(IConfiguration configuration, ApiLogger apiLogger, SqlResult sqlResult)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods
        public Response SearchReference(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand($"SELECT * FROM [DTCData] WHERE ReferenceNumber = '{referenceNumber}'", sql);
                    return _sqlResult.DataExists(cmd, sql);
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "PdfConsultasDb: SearchReference", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
       
        public DataSet GetStorePDF(string clavePlaza, string numeroReferencia, string inicialRef)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DTCtoPDFPrueba", sql))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();


                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = numeroReferencia;
                        cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = inicialRef;

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
                _apiLogger.WriteLog(clavePlaza, ex, "PdfConsultasDb: GetStorePDF", 1);
                return null;
            }
        }

        public DataSet GetStorePDFOpen(string clavePlaza, string numeroReferencia)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spDTCtoPDFOpen", sql))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();


                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = numeroReferencia;

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
                _apiLogger.WriteLog(clavePlaza, ex, "PdfConsultasDb: GetStorePDFOpen", 1);
                return null;
            }
        }
        #endregion
    }
}
