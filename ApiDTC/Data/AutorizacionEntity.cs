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
    public class AutorizacionEntity
    {
        private readonly string _connectionString;

        public AutorizacionEntity(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }

        public Autorizacion GetPieAutorizacion(string fecha)
        {
            Autorizacion entidad = new Autorizacion();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string spName = @"dbo.spAutorizacion";
                    SqlCommand cmd = new SqlCommand(spName, conn);
                    conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@fecha", SqlDbType.Date).Value = Convert.ToDateTime(fecha);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            entidad.LabelAutorizacion = dr.GetString(0);
                            entidad.NameAutorizacion = dr.GetString(1);
                            entidad.FirmaImagen = dr.GetString(2);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No data found.");
                    }
                    dr.Close();
                    conn.Close();
                }
                return entidad;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}