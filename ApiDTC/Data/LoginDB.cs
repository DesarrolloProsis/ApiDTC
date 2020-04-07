namespace ApiDTC.Data
{
    using Models;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    
    public class LoginDb
    {

        private readonly string _connectionString;

        public LoginDb(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }


        public List<SelectListItem> GetTec(string numPlaza)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_TecnicosPlaza", sql))
                {

                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@PlazaId", SqlDbType.NVarChar).Value = numPlaza;

                        sql.Open();



                        var response = new List<SelectListItem>();
                        var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            response.Add(new SelectListItem
                            {
                                //Value = reader["ComponentsStockId"].ToString(),
                                Value = Convert.ToString(reader["UserId"]),
                                Text = Convert.ToString(reader["TecnicosAsignados"])

                            });
                        }

                        return response;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }
        }
        public List<Login> GetHeadTec(int idTec)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_LiquidacionTerceroHeader", sql))
                {


                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@user", SqlDbType.Int).Value = idTec;
             

                        sql.Open();



                        var response = new List<Login>();
                        var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            response.Add(MapToLogin(reader));
                        }

                        return response;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }
        }

        public List<Login> GetStoreLogin(string nombreUsuario, string passWord, bool flag)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_Login", sql))
                {


                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar).Value = nombreUsuario;
                        cmd.Parameters.Add("@Contraseña", SqlDbType.NVarChar).Value = passWord;
                        cmd.Parameters.Add("@Flag", SqlDbType.Bit).Value = flag;
                        
                        sql.Open();



                        var response = new List<Login>();
                        var reader = cmd.ExecuteReader();
                        
                            while (reader.Read())
                            {
                                response.Add(MapToLogin(reader));
                            }
                        
                        return response;
                    }
                    catch(Exception ex)
                    {                        
                        return null;
                    }
                }
            }
        }
        public List<Cookie> GetStoreLoginCokie(string nombreUsuario, string passWord, bool flag)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_Login", sql))
                {


                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar).Value = nombreUsuario;
                        cmd.Parameters.Add("@Contraseña", SqlDbType.NVarChar).Value = passWord;
                        cmd.Parameters.Add("@Flag", SqlDbType.Bit).Value = flag;

                        sql.Open();

                        var response = new List<Cookie>();
                        var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            response.Add(MapToCokie(reader));
                        }

                        return response;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }
        }

        private Login MapToLogin(SqlDataReader reader)
        {
            return new Login()
            {
                UserId = (int)reader["UserId"],
                AgremmentInfoId = (int)reader["AgremmentInfoId"],
                Nombre = reader["Nombre"].ToString(),
                Plaza = reader["Plaza"].ToString(),
                Agrement = reader["Agrement"].ToString(),
                ManagerName = reader["ManagerName"].ToString(),
                Position = reader["Position"].ToString(),
                Mail = reader["Mail"].ToString(),
                AgremmentDate = Convert.ToDateTime(reader["AgremmentDate"].ToString()),
                DelegationName = reader["DelegationName"].ToString(),
                RegionalCoordination = reader["RegionalCoordination"].ToString(),
            };
        }

        private Cookie MapToCokie(SqlDataReader reader)
        {
            return new Cookie()
            {
                UserId =  (int)reader["UserId"],
                SquareCatalogId = Convert.ToString(reader["SquareCatalogId"]),
                RollId = (int)reader["RollId"]
            };
        }
    }

}
