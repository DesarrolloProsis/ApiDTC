namespace ApiDTC.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using ApiDTC.Models;

    public class SqlResult
    {
<<<<<<< HEAD
        private string _propertyError;
        private string _classMapped;
=======
        #region Attributes
>>>>>>> 8d96d5597f1a9207494859e40cea9ef7a082d5f0
        private ApiLogger _apiLogger;

        private string _classMapped, _propertyMapped;
        #endregion
        public SqlResult(ApiLogger apiLogger)
        {
            _apiLogger = apiLogger;
        }

        public InsertResponse Post(SqlCommand cmd, SqlConnection con)
        {
            try
            {
                con.Open();
                if(con.State != ConnectionState.Open)
                {
                    return new InsertResponse
                    {
                        SqlMessage = "SQL connection is closed",
                        SqlResult = null
                    };
                }
                var reader = cmd.ExecuteReader();
                if(!reader.HasRows)
                {
                    return new InsertResponse
                    {
                        SqlMessage = "Result not found",
                        SqlResult = null
                    };
                }
                var result = PostMapper(reader);
                con.Close();
                return (InsertResponse)result;   
            }
            catch(SqlException ex)
            {
                _apiLogger.WriteLog(ex, "Post");
                return new InsertResponse
                {
                    SqlMessage = $"Error: {ex.Message}",
                    SqlResult = null
                    
                };
            }    
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
                var result = GetMapper<T>(reader);
                con.Close();
                return (Response)result;   
            }
            catch(SqlException ex)
            {
                _apiLogger.WriteLog(ex, "GetList<T>");
                return new Response
                {
                    Message = $"Error: {ex.Message}.",
                    Result = null
                    
                };
            }       
        }

        private InsertResponse PostMapper(SqlDataReader rdr)
        {
            
            try
            {
                var obj = new InsertResponse();
                if(rdr.Read())
                {
                    _propertyMapped = obj.GetType().Name;
                    foreach (PropertyInfo p in obj.GetType().GetProperties())
                    {
                        _propertyMapped = p.Name;
                        p.SetValue(obj, rdr[p.Name], null);
                    };
                }
                _propertyMapped = null;
                return obj;
            }
            catch(Exception ex)
            {
                _apiLogger.WriteLog(ex, $"PostMapper. La clase InsertResponse no pudo ser mapeada en propiedad {_propertyMapped}");
                _propertyMapped = null;
                return new InsertResponse
                {
                    SqlMessage = $"Error: {ex.Message}",
                    SqlResult = null
                };
            } 
        }

        private Response GetMapper<T>(SqlDataReader rdr)
        {
            try
            {
                var list = new List<T>();
                T obj = default(T);
                _classMapped = nameof(obj);
                int rows = 0; 
                while(rdr.Read())
                {
                    rows++;
                    obj = Activator.CreateInstance<T>();
                    _propertyMapped = obj.GetType().Name;
                    foreach (PropertyInfo p in obj.GetType().GetProperties())
                    {
<<<<<<< HEAD
                        _propertyError = p.Name;
=======
                        _propertyMapped = p.Name;
>>>>>>> 8d96d5597f1a9207494859e40cea9ef7a082d5f0
                        p.SetValue(obj, rdr[p.Name], null);
                    }
                    list.Add(obj);
                }
                _propertyMapped = null;
                _classMapped = null;
                return new Response
                {
                    Message = "Ok",
                    Result = list,
                    Rows = rows
                };
            }
            catch(Exception ex)
            {
<<<<<<< HEAD
                _apiLogger.WriteLog(ex, $"Mapper Clase: {_classMapped} no mapeada en propiedad {_propertyError}");
=======
                _apiLogger.WriteLog(ex, $"Mapper. La clase {_classMapped} no pudo ser mapeada en propiedad {_propertyMapped}");
                _propertyMapped = null;
                _classMapped = null;
>>>>>>> 8d96d5597f1a9207494859e40cea9ef7a082d5f0
                return new Response
                {
                    Message = $"Error: {ex.Message} Clase: {_classMapped} no mapeada en propiedad {_propertyError}",
                    Result = null
                };
            } 
        }
    }
}