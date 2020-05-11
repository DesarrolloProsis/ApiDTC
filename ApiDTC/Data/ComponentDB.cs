namespace ApiDTC.Data
{
    using Models;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using ApiDTC.Services;

    public class ComponentDb
    {
        #region Attributes
        private readonly string _connectionString;

        private SqlResult _sqlResult;
        #endregion

        #region Constructor
        public ComponentDb(IConfiguration configuration, SqlResult sqlResult)
        {
            _sqlResult = sqlResult;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods

        //TODO Mapper foreach var lane
        public Response GetComponentData(string convenio, string plaza, string Id, string marca)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_ComponentInfoDTC", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Agremmnt", SqlDbType.NVarChar).Value = convenio;
                    cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = plaza;
                    cmd.Parameters.Add("@Component", SqlDbType.NVarChar).Value = Id;
                    
                    var storedResult = _sqlResult.GetList<Components>(cmd, sql);
                    if(storedResult.Result == null)
                        return storedResult;
                    var list = (List<Components>)storedResult.Result;
                    string[] listLane = new string[list.Count];
                    int i = 0;

                    List<Components> listaFiltro = new List<Components>();



                    foreach (var item in list)
                    {
                        if(item.Brand == marca)
                        {
                            listLane[i++] = item.Lane;
                            listaFiltro.Add(item);
                        }
                        
                    }
                    storedResult.Result = new { listaFiltro, listLane };
                    return new Response
                    {
                        Message = "Ok",
                        Result = storedResult.Result
                    };
                }
            }
        }

        //Revisar
        public Response GetComponentsData(int plaza, string numConvenio)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand($"select a.Component as Description, a.Brand as Brand from SquareInventory a join LanesCatalog b on (a.CapufeLaneNum = b.CapufeLaneNum and a.IdGare = b.IdGare) join ComponentsStock c on a.Component = c.Description join AgreementInfo d on c.AgremmentInfoId = d.AgremmentInfoId where a.Brand != 'NO APLICA' and b.SquareCatalogid = '{plaza}' and d.Agrement = '{numConvenio}' group by a.Component, a.Brand", sql);
                return _sqlResult.GetList<ComponentsDescription>(cmd, sql);
            }
        }
        #endregion
    }
}
