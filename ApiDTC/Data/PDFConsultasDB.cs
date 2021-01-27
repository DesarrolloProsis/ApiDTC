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

        private readonly ApiLogger _apiLogger;

        private readonly SqlResult _sqlResult;
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
                    return _sqlResult.DataExists(clavePlaza, cmd, sql, "SearchReference");
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "PdfConsultasDb: SearchReference", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public SqlResponse FirmarReporte(string clavePlaza, string referenceNumber)
        {
            try
            {
                using(SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using(SqlCommand cmd = new SqlCommand("spUpdateStatusDTC", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;
                        cmd.Parameters.Add("@status", SqlDbType.Int).Value = 2;
                        return _sqlResult.Put(clavePlaza, cmd, sql, "FirmarReporte");
                    }
                }
            }
            catch (SqlException ex)
            {

                _apiLogger.WriteLog(clavePlaza, ex, "PdfConsultasDb: TerminarReporte", 1);
                return new SqlResponse { SqlMessage = $"Error: {ex.Message}", SqlResult = null };
            }
        }

        public Response AutorizadoGmmp(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spUpdateStatusDTC", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;
                        cmd.Parameters.Add("@status", SqlDbType.Int).Value = 4;
                        var result = _sqlResult.Put(clavePlaza, cmd, sql, "AutorizadoGmmp");
                        return new Response
                        {
                            Message = result.SqlMessage,
                            Result = result.SqlResult
                        };
                    }
                }
            }
            catch (SqlException ex)
            {

                _apiLogger.WriteLog(clavePlaza, ex, "PdfConsultasDb: AutorizadoGmmp", 1);
                return new Response
                {
                    Message = $"Error: {ex.Message}",
                    Result = null
                };
            }
        }

        public Response UpdateStatus(string clavePlaza, string referenceNumber, int status)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spUpdateStatusDTC", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;
                        cmd.Parameters.Add("@status", SqlDbType.Int).Value = status;
                        var result = _sqlResult.Put(clavePlaza, cmd, sql, "UpdateStatus");
                        return new Response
                        {
                            Message = result.SqlMessage,
                            Result = result.SqlResult
                        };
                    }
                }
            }
            catch (SqlException ex)
            {

                _apiLogger.WriteLog(clavePlaza, ex, "PdfConsultasDb: UpdateStatus", 1);
                return new Response
                {
                    Message = $"Error: {ex.Message}",
                    Result = null
                };
            }
        }

        public Response UpdateStatusAdmin(string clavePlaza, DtcStatusLog dtcStatusLog)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spUpdateDTCStatusLog", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = dtcStatusLog.ReferenceNumber;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = dtcStatusLog.UserId;
                        cmd.Parameters.Add("@Status", SqlDbType.Int).Value = dtcStatusLog.StatusId;
                        cmd.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = dtcStatusLog.Comment;
                        var result = _sqlResult.Put(clavePlaza, cmd, sql, "UpdateStatusAdmin");
                        return new Response
                        {
                            Message = result.SqlMessage,
                            Result = result.SqlResult
                        };
                    }
                }
            }
            catch (SqlException ex)
            {

                _apiLogger.WriteLog(clavePlaza, ex, "PdfConsultasDb: UpdateStatusAdmin", 1);
                return new Response
                {
                    Message = $"Error: {ex.Message}",
                    Result = null
                };
            }
        }

        public SqlResponse SelladoReporte(string clavePlaza, string referenceNumber)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spUpdateStatusDTC", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = referenceNumber;
                        cmd.Parameters.Add("@status", SqlDbType.Int).Value = 3;
                        return _sqlResult.Put(clavePlaza, cmd, sql, "FirmarReporte");
                    }
                }
            }
            catch (SqlException ex)
            {

                _apiLogger.WriteLog(clavePlaza, ex, "PdfConsultasDb: TerminarReporte", 1);
                return new SqlResponse { SqlMessage = $"Error: {ex.Message}", SqlResult = null };
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

        //Para PDF
        public DataSet GetStorePDFMetraje(string clavePlaza, string numeroReferencia, string inicialRef)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[sp_DTCtoPDFPruebaMetraje]", sql))
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
