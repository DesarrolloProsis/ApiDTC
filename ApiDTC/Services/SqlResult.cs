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
        #region Attributes
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
                if (con.State != ConnectionState.Open)
                {
                    return new InsertResponse
                    {
                        SqlMessage = "SQL connection is closed",
                        SqlResult = null
                    };
                }
                var reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    return new InsertResponse
                    {
                        SqlMessage = "No se pudo insertar el registro",
                        SqlResult = null
                    };
                }
                var result = PostMapper(reader);
                con.Close();
                return result;
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(ex, "Post");
                return new InsertResponse
                {
                    SqlMessage = $"Error: {ex.Message}",
                    SqlResult = null

                };
            }
        }

        public Response DataExists(SqlCommand command, SqlConnection con)
        {
            try
            {
                con.Open();
                if (con.State != ConnectionState.Open)
                {
                    return new Response
                    {
                        Message = "SQL connection is closed",
                        Result = null
                    };
                }
                var reader = command.ExecuteReader();
                if (!reader.HasRows)
                {
                    return new Response
                    {
                        Message = "Result not found",
                        Result = null
                    };
                }
                return new Response
                {
                    Message = "Result found",
                    Result = true
                };
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(ex, "GetList<T>");
                return new Response
                {
                    Message = $"Error: {ex.Message}",
                    Result = null

                };
            }
        }
        public Response GetList<T>(SqlCommand command, SqlConnection con)
        {
            try
            {
                con.Open();
                if (con.State != ConnectionState.Open)
                {
                    return new Response
                    {
                        Message = "SQL connection is closed",
                        Result = null
                    };
                }
                var reader = command.ExecuteReader();
                if (!reader.HasRows)
                {
                    return new Response
                    {
                        Message = "Result not found",
                        Result = null
                    };
                }
                var result = GetMapper<T>(reader);
                con.Close();
                return result;
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(ex, "GetList<T>");
                return new Response
                {
                    Message = $"Error: {ex.Message}",
                    Result = null

                };
            }
        }

        private InsertResponse PostMapper(SqlDataReader rdr)
        {
            try
            {
                var obj = new InsertResponse();
                if (rdr.Read())
                {
                    if(rdr.IsDBNull(0))
                        obj.SqlResult = null;
                    else
                        obj.SqlResult = rdr[0];

                    obj.SqlMessage = rdr["SqlMessage"].ToString();
                    _propertyMapped = obj.GetType().Name;
                }
                _propertyMapped = null;
                return obj;
            }
            catch (Exception ex)
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
                int rows = 0;
                while (rdr.Read())
                {
                    rows++;
                    obj = Activator.CreateInstance<T>();
                    _propertyMapped = obj.GetType().Name;
                    foreach (PropertyInfo p in obj.GetType().GetProperties())
                    {
                        _propertyMapped = p.Name;
                        if (!DBNull.Value.Equals(rdr[p.Name]))
                            p.SetValue(obj, rdr[p.Name], null);
                        else
                            p.SetValue(obj, null, null);
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
            catch (Exception ex)
            {
                _apiLogger.WriteLog(ex, $"Mapper. La clase {_classMapped} no pudo ser mapeada en propiedad {_propertyMapped}");
                _propertyMapped = null;
                _classMapped = null;
                return new Response
                {
                    Message = $"Error: {ex.Message}",
                    Result = null
                };
            }
        }
    }
}