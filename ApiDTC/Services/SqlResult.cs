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
        private readonly ApiLogger _apiLogger;


        private string _propertyMapped;
        #endregion

        #region Constructor
        public SqlResult(ApiLogger apiLogger)
        {
            _apiLogger = apiLogger;
        }
        #endregion

        #region Methods
        public SqlResponse Post(string clavePlaza, SqlCommand cmd, SqlConnection con, string origen)
        {
            try
            {
                con.Open();
                if (con.State != ConnectionState.Open)
                {
                    return new SqlResponse
                    {
                        SqlMessage = "SQL connection is closed",
                        SqlResult = null
                    };
                }
                var reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                        return new SqlResponse
                    {
                        SqlMessage = "No se pudo insertar el registro",
                        SqlResult = null
                    };
                }
                var result = PostMapper(clavePlaza, reader, origen);
                con.Close();
                return result;
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, $"SqlResult: Post", 1);
                return new SqlResponse
                {
                    SqlMessage = $"Error: {ex.Message}",
                    SqlResult = null

                };
            }
        }
        
       
        public SqlResponse Put(string clavePlaza, SqlCommand cmd, SqlConnection con, string origen)
        {
            try
            {
                con.Open();
                if (con.State != ConnectionState.Open)
                {
                    return new SqlResponse
                    {
                        SqlMessage = "SQL connection is closed",
                        SqlResult = null
                    };
                }
                var reader = cmd.ExecuteReader();
                
                if (!reader.HasRows)
                {
                    return new SqlResponse
                    {
                        SqlMessage = "No se pudo insertar el registro",
                        SqlResult = null
                    };
                }
                var result = PostMapper(clavePlaza, reader, origen);
                con.Close();
                return result;
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, $"{origen}: Put", 1);
                return new SqlResponse
                {
                    SqlMessage = $"Error: {ex.Message}",
                    SqlResult = null

                };
            }
        }
        
        public Response DataExists(string clavePlaza, SqlCommand command, SqlConnection con, string origen)
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
                con.Close();
                return new Response
                {
                    Message = "Result found",
                    Result = true
                };
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, $"{origen}: DataExists", 1);
                return new Response
                {
                    Message = $"Error: {ex.Message}",
                    Result = null

                };
            }
        }

        public T GetRow<T>(string clavePlaza, SqlCommand command, SqlConnection con, string origen)
        {
            try
            {
                con.Open();
                T obj = default;
                if (con.State != ConnectionState.Open)
                {
                    return default(T);
                }
                int rows = 0;
                var reader = command.ExecuteReader();
                if(reader.Read())
                {
                    rows++;
                    obj = Activator.CreateInstance<T>();
                    _propertyMapped = obj.GetType().Name;
                    foreach (PropertyInfo p in obj.GetType().GetProperties())
                    {
                        _propertyMapped = p.Name;
                        if(!DBNull.Value.Equals(reader[p.Name]))
                            p.SetValue(obj, reader[p.Name], null);
                        else
                            p.SetValue(obj, null, null);
                    }
                }
                con.Close();
                _propertyMapped = null;
                return obj;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, $"{origen}: GetList<T>", 3);
                _propertyMapped = null;
                return default(T);
            }
        }

        public List<T> GetRows<T>(string clavePlaza, SqlCommand command, SqlConnection con, string origen)
        {
            try
            {
                con.Open();
                var list = new List<T>();
                T obj = default;
                if (con.State != ConnectionState.Open)
                    return null;
                int rows = 0;
                var reader = command.ExecuteReader();
                while(reader.Read())
                {
                    rows++;
                    obj = Activator.CreateInstance<T>();
                    _propertyMapped = obj.GetType().Name;
                    foreach (PropertyInfo p in obj.GetType().GetProperties())
                    {
                        _propertyMapped = p.Name;
                        if(!DBNull.Value.Equals(reader[p.Name]))
                            p.SetValue(obj, reader[p.Name], null);
                        else
                            p.SetValue(obj, null, null);
                    }
                    list.Add(obj);
                }
                con.Close();
                _propertyMapped = null;
                return list;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, $"{origen}: GetList<T>", 3);
                _propertyMapped = null;
                return null;
            }
        }


        public T GetRow<T>(string clavePlaza, DataTable dataTable, string origen)
        {
            try
            {
                T obj = default;
                int rows = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    rows++;
                    obj = Activator.CreateInstance<T>();
                    _propertyMapped = obj.GetType().Name;
                    foreach (PropertyInfo p in obj.GetType().GetProperties())
                    {
                        _propertyMapped = p.Name;
                        if(!DBNull.Value.Equals(row[p.Name]))
                            p.SetValue(obj, row[p.Name], null);
                        else
                            p.SetValue(obj, null, null);
                    }
                }
                _propertyMapped = null;
                return obj;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, $"{origen}: GetList<T>", 3);
                _propertyMapped = null;
                return default(T);
            }
        }

        public List<T> GetRows<T>(string clavePlaza, DataTable dataTable, string origen)
        {
            try
            {
                var list = new List<T>();
                T obj = default;
                int rows = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    rows++;
                    obj = Activator.CreateInstance<T>();
                    _propertyMapped = obj.GetType().Name;
                    foreach (PropertyInfo p in obj.GetType().GetProperties())
                    {
                        _propertyMapped = p.Name;
                        if(!DBNull.Value.Equals(row[p.Name]))
                            p.SetValue(obj, row[p.Name], null);
                        else
                            p.SetValue(obj, null, null);
                        list.Add(obj);
                    }
                }
                _propertyMapped = null;
                return list;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, $"{origen}: GetList<T>", 3);
                _propertyMapped = null;
                return null;
            }
        }
        
        public Response GetList<T>(string clavePlaza, SqlCommand command, SqlConnection con, string origen)
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
                var result = GetMapper<T>(clavePlaza, reader, origen);
                con.Close();
                return result;
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, $"{origen}: GetList<T>", 1);
                return new Response
                {
                    Message = $"Error: {ex.Message}",
                    Result = null

                };
            }
        }

        public List<T> DataSetMapper<T>(string clavePlaza, DataTable dataSet, string origen)
        {
            try
            {
                var list = new List<T>();
                T obj = default;
                foreach(DataRow row in dataSet.Rows)
                {
                    obj = Activator.CreateInstance<T>();
                    _propertyMapped = obj.GetType().Name;
                    foreach (PropertyInfo p in obj.GetType().GetProperties())
                    {
                        _propertyMapped = p.Name;
                        if (!DBNull.Value.Equals(row[p.Name]))
                            p.SetValue(obj, row[p.Name], null);
                        else
                            p.SetValue(obj, null, null);
                    }
                    list.Add(obj);
                }
                _propertyMapped = null;
                return list;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, $"{origen}: DataSetMapper<T>", 3);
                _propertyMapped = null;
                return null;
            }
        }

        private SqlResponse PostMapper(string clavePlaza, SqlDataReader rdr, string origen)
        {
            try
            {
                var obj = new SqlResponse();
                if (rdr.Read())
                {
                    if(rdr.IsDBNull(0))
                        obj.SqlResult = null;
                    else
                        obj.SqlResult = rdr[0];

                    obj.SqlMessage = rdr["SqlMessage"].ToString();
                }
                return obj;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, $"{origen}: PostMapper", 3);
                _propertyMapped = null;
                return new SqlResponse
                {
                    SqlMessage = $"Error: {ex.Message}",
                    SqlResult = null
                };
            }
        }
        
        private Response GetMapper<T>(string clavePLaza, SqlDataReader rdr, string origen)
        {
            try
            {
                var list = new List<T>();
                T obj = default;
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
                return new Response
                {
                    Message = "Ok",
                    Result = list,
                    Rows = rows
                };
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(clavePLaza, ex, $"{origen}: GetMapper<T>", 3);
                _propertyMapped = null;
                return new Response
                {
                    Message = $"Error: {ex.Message}",
                    Result = null
                };
            }
        }

        public SqlResponse Count(string clavePlaza, SqlCommand cmd, SqlConnection con, string origen)
        {
            try
            {
                con.Open();
                if (con.State != ConnectionState.Open)
                {
                    return new SqlResponse
                    {
                        SqlMessage = "SQL connection is closed",
                        SqlResult = null
                    };
                }
                int reader = (Int32)cmd.ExecuteScalar();
               
                //var result = PostMapper(clavePlaza, reader, origen);
                con.Close();
                return new SqlResponse
                {
                    SqlMessage = $"Ok",
                    SqlResult = reader

                };
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, $"SqlResult: Post", 1);
                return new SqlResponse
                {
                    SqlMessage = $"Error: {ex.Message}",
                    SqlResult = null

                };
            }
        }

        #endregion
    }
}