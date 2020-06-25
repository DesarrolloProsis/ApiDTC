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
                    if (storedResult.Result == null)
                        return storedResult;
                    var list = (List<Components>)storedResult.Result;
                    string[] listLane = new string[list.Count];
                    int i = 0;

                    List<Components> listaFiltro = new List<Components>();

                    foreach (var item in list)
                    {
                        if (item.Brand == marca)
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
   

        public Response PutComponentInventary(UpdateInventory updateInventory)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_UpdateInventory", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@strFltLane", SqlDbType.NVarChar).Value = updateInventory.strFltLane;
                    cmd.Parameters.Add("@strFltComponent", SqlDbType.NVarChar).Value = updateInventory.strFltComponent;
                    cmd.Parameters.Add("@strFltSerialNumber", SqlDbType.NVarChar).Value = updateInventory.strFltSerialNumber;
                    cmd.Parameters.Add("@strFltSquare", SqlDbType.NVarChar).Value = updateInventory.strFltSquare;
                    cmd.Parameters.Add("@strInventaryNumCapufe", SqlDbType.NVarChar).Value = updateInventory.strInventaryNumCapufe;
                    cmd.Parameters.Add("@strInventaryNumProsis", SqlDbType.NVarChar).Value = updateInventory.strInventaryNumProsis;
                    cmd.Parameters.Add("@strMorel", SqlDbType.NVarChar).Value = updateInventory.strMorel;
                    cmd.Parameters.Add("@strBrand", SqlDbType.NVarChar).Value = updateInventory.strBrand;
                    cmd.Parameters.Add("@strSerialNumber", SqlDbType.NVarChar).Value = updateInventory.strSerialNumber;
                    cmd.Parameters.Add("@strInstalationDate", SqlDbType.NVarChar).Value = updateInventory.strInstalationDate;
                    cmd.Parameters.Add("@strObservation", SqlDbType.NVarChar).Value = updateInventory.strObservation;
                    cmd.Parameters.Add("@intUbicacion", SqlDbType.Int).Value = updateInventory.intUbicacion;
                    cmd.Parameters.Add("@strMaintenanceDate", SqlDbType.NVarChar).Value = updateInventory.strMaintenanceDate;
                    cmd.Parameters.Add("@strMaintenanceFolio", SqlDbType.NVarChar).Value = updateInventory.strMaintenanceFolio;


                    var reader = _sqlResult.Put(cmd, sql);
                    if(reader.SqlResult == null)
                    {
                        return new Response
                        {
                            Message = "Fail",
                            Result = updateInventory

                        };
                    }
                    return new Response
                    {
                        Message = "Ok",
                        Result = updateInventory
                    };
                }
            }
        }


        //Revisar
        public Response GetComponentsData(string plaza, string numConvenio)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand($"select a.Component as Description, a.Brand as Brand from SquareInventory a join LanesCatalog b on (a.CapufeLaneNum = b.CapufeLaneNum and a.IdGare = b.IdGare) join ComponentsStock c on a.Component = c.Description join AgreementInfo d on c.AgremmentInfoId = d.AgremmentInfoId where a.Brand != 'NO APLICA' and b.SquareCatalogid = '{plaza}' and d.Agrement = '{numConvenio}' group by a.Component, a.Brand", sql);
                return _sqlResult.GetList<ComponentsDescription>(cmd, sql);
            }
        }

        public Response GetComponentsInventory(string squareId)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand($"select Component Componente"+
    " from SquareInventory a join LanesCatalog b on (a.CapufeLaneNum = b.CapufeLaneNum and a.IdGare = b.IdGare)"+
    $" where b.SquareCatalogId = {squareId}"+
    " group by Component", sql);
                return _sqlResult.GetList<ComponentsInventory>(cmd, sql);
            }
        }


        public Response GetComponentsInventoryUbication()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand($"select TypeUbicationId,Name Ubicacion from TypesUbication", sql);
                return _sqlResult.GetList<ComponentsInventoryUbication>(cmd, sql);
            }
        }

        public Response GetComponentsInventoryLane(string Component,string squareId)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand($"select b.Lane "+
                    "from SquareInventory a "+
                    "join LanesCatalog b "+
                    "on (a.CapufeLaneNum = b.CapufeLaneNum and a.IdGare = b.IdGare) "+
                    $"where a.Component = '{Component}' and b.SquareCatalogId = '{squareId}' " +
                    "group by b.Lane", sql);
                return _sqlResult.GetList<ComponentsInventoryLane>(cmd, sql);
            }
        }

        public Response GetComponentsInventoryDescription(string Component, string Lane, string squareId)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand($"select InventaryNumCapufe, " +
                       "InventaryNumProsis, " +
                       "Model, " +
                       "Brand, " +
                       "SerialNumber, " +
                       "cast(InstalationDate as nvarchar) InstalationDate, " +
                       "Observations, " +
                       "c.Name Ubication, " +
                       "cast(MaintenanceDate as nvarchar) MaintenanceDate, " +
                       "MaintenanceFolio " +
                    "from SquareInventory a join LanesCatalog b " +
                        "on (a.CapufeLaneNum = b.CapufeLaneNum and a.IdGare = b.IdGare) " +
                    "join TypesUbication c on a.TypeUbicationId = c.TypeUbicationId " +
                    $"where Component = '{Component}' and b.Lane = '{Lane}' and b.SquareCatalogId = {squareId}", sql);
                return _sqlResult.GetList<ComponentsInventoryDescription>(cmd, sql);
            }
        }


        #endregion
    }
}
