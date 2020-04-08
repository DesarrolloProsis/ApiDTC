﻿namespace ApiDTC.Data
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
        #endregion

        #region Constructor
        public PdfConsultasDb(IConfiguration configuration, ApiLogger apiLogger)
        {
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods
        //TODO Check procedure
        public DataSet GetStorePDF(string numeroReferencia)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DTCtoPDF", sql))
                {

                    try
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
                    catch (Exception ex)
                    {
                        _apiLogger.WriteLog(ex, "GetStorePDF");
                        return null;   
                    }
                }
            }
        }

        public string ConverToMoneda(string value)
        {
            Moneda moneda = new Moneda();
            return moneda.Convertir(value, true);

        }
        #endregion
    }
}
