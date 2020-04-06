using ApiDTC.Models;
using ApiDTC.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Data
{
    public class PDFConsultasDB
    {
        private SqlMapper _sqlMapper;

        private readonly string _connectionString;

        public PDFConsultasDB(IConfiguration configuration, SqlMapper sqlMapper)
        {
            _sqlMapper = sqlMapper;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }


        public DataSet GetStorePDF(string numeroReferencia)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_header", sql))
                {

                    var header = new PdfHeader();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ReferenceNumber", SqlDbType.NVarChar).Value = numeroReferencia;

                    sql.Open();
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if(reader.HasRows)
                            header = _sqlMapper.ConvertToObject<PdfHeader>(reader);
                    }
                }
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
                        return null;   
                    }
                }
            }
        }
    }
}
