namespace ApiDTC.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Reflection;
    using ApiDTC.Models;

    public class SqlResult
    {
        private ApiLogger _apiLogger;

        public SqlResult(ApiLogger apiLogger)
        {
            _apiLogger = apiLogger;
        }

        public Response GetList<T>(SqlCommand command, SqlConnection con)
        {
            try
            {
                con.Open();
                if(con.State != ConnectionState.Open)
                {
                    return new Response
                    {
                        Message = "SQL connection is closed",
                        Result = null
                    };
                }
                var reader = command.ExecuteReader();
                if(!reader.HasRows)
                {
                    return new Response
                    {
                        Message = "Result not found",
                        Result = null
                    };
                }
                var result = Mapper<T>(reader);
                con.Close();
                return result;   
            }
            catch(SqlException ex)
            {
                _apiLogger.WriteLog(ex, "GetList<T>");
                return new Response
                {
                    Message = $"Error: {ex.Message}",
                    Result = null
                    
                };
            }       
        }

        private Response Mapper<T>(SqlDataReader rdr)
        {
            try
            {
                var list = new List<T>();
                T obj = default(T);
                int rows = 0; 
                while(rdr.Read())
                {
                    ++rows;
                    obj = Activator.CreateInstance<T>();
                    foreach (PropertyInfo p in obj.GetType().GetProperties())
                        p.SetValue(obj, rdr[p.Name], null);
                    list.Add(obj);
                }
                return new Response
                {
                    Message = "Ok",
                    Result = list, 
                    Rows = rows
                };
            }
            catch(Exception ex)
            {
                _apiLogger.WriteLog(ex, "Mapper");
                return new Response
                {
                    Message = $"Error: {ex.Message}",
                    Result = null
                };
            } 
        }
    }
}