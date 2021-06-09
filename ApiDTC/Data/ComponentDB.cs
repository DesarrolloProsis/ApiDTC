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

        private readonly SqlResult _sqlResult;

        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public ComponentDb(IConfiguration configuration, SqlResult sqlResult, ApiLogger apiLogger)
        {
            _sqlResult = sqlResult;
            _apiLogger = apiLogger;
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        #endregion

        #region Methods

        public Response GetComponentDataModificaciones(string clavePlaza, string squareId, int agreementId, int attachedId, int relationShip, int relationShipPrincipal)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_ComponentInfoDTCVer2", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = squareId;
                        cmd.Parameters.Add("@AgreementInt", SqlDbType.Int).Value = agreementId;
                        cmd.Parameters.Add("@AttachedId", SqlDbType.Int).Value = attachedId;
                        cmd.Parameters.Add("@ComponentsRelationship", SqlDbType.Int).Value = relationShip;
                        cmd.Parameters.Add("@MainComponentsRelationship", SqlDbType.Int).Value = relationShipPrincipal;

                        var storedResult = _sqlResult.GetList<Components>(clavePlaza, cmd, sql, "GetComponentDataModificaciones");
                        if (storedResult.Result == null)
                            return storedResult;
                        var list = (List<Components>)storedResult.Result;
                        string[] listLane = new string[list.Count];
                        int i = 0;

                        List<Components> listaFiltro = new List<Components>();

                        foreach (var item in list)
                        {
                            listLane[i++] = item.Lane;
                            listaFiltro.Add(item);
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
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ComponentDb: GetComponentDataModificaciones", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        
        public Response GetComponentData(string clavePlaza, string convenio, string plaza, string Id, string marca)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_ComponentInfoDTC", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Agremmnt", SqlDbType.NVarChar).Value = convenio;
                        cmd.Parameters.Add("@SquareId", SqlDbType.NVarChar).Value = plaza;
                        cmd.Parameters.Add("@Component", SqlDbType.NVarChar).Value = Id;

                        var storedResult = _sqlResult.GetList<Components>(clavePlaza, cmd, sql, "GetComponentData");
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
            catch (SqlException ex)
            {

                _apiLogger.WriteLog(clavePlaza, ex, "ComponentDb: GetComponentData", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
        
        public Response VersionPruebaComponet(string clavePlaza, string plaza, string numConvenio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand($"select a.Component as Description, a.Brand as Brand from SquareInventory a join LanesCatalog b on (a.CapufeLaneNum = b.CapufeLaneNum and a.IdGare = b.IdGare) join ComponentsStock c on a.Component = c.Description join AgreementInfo d on c.AgremmentInfoId = d.AgremmentInfoId where a.Brand != 'NO APLICA' and b.SquareCatalogid = '{plaza}' and d.Agrement = '{numConvenio}' group by a.Component, a.Brand", sql);
                    return _sqlResult.GetList<ComponentsDescription>(clavePlaza, cmd, sql, "VersionPruebaComponent");
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ComponentDb: VersionPruebaComponet", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response PutComponentInventary(string clavePlaza, UpdateInventory updateInventory)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_UpdateInventory", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@intFltrId", SqlDbType.NVarChar).Value = updateInventory.TableFolio;
                        cmd.Parameters.Add("@strInventaryNumCapufe", SqlDbType.NVarChar).Value = updateInventory.strInventaryNumCapufe;
                        cmd.Parameters.Add("@strInventaryNumProsis", SqlDbType.NVarChar).Value = updateInventory.strInventaryNumProsis;
                        cmd.Parameters.Add("@strModel", SqlDbType.NVarChar).Value = updateInventory.strMorel;
                        cmd.Parameters.Add("@strBrand", SqlDbType.NVarChar).Value = updateInventory.strBrand;
                        cmd.Parameters.Add("@strSerialNumber", SqlDbType.NVarChar).Value = updateInventory.strSerialNumber;
                        cmd.Parameters.Add("@strInstalationDate", SqlDbType.NVarChar).Value = updateInventory.strInstalationDate;
                        cmd.Parameters.Add("@strObservation", SqlDbType.NVarChar).Value = updateInventory.strObservation;
                        cmd.Parameters.Add("@intUbicacion", SqlDbType.Int).Value = updateInventory.intUbicacion;
                        cmd.Parameters.Add("@strMaintenanceDate", SqlDbType.NVarChar).Value = updateInventory.strMaintenanceDate;
                        cmd.Parameters.Add("@strMaintenanceFolio", SqlDbType.NVarChar).Value = updateInventory.strMaintenanceFolio;
                        cmd.Parameters.Add("@intUserId", SqlDbType.Int).Value = updateInventory.intUserId;

                        var reader = _sqlResult.Put(clavePlaza, cmd, sql, "PutComponentInventary");
                        if (reader.SqlResult == null)
                        {
                            return new Response
                            {
                                Message = "Fail",
                                Result = null

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
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ComponentDb: PutComponentInventary", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response InventoryListUpdate(string clavePlaza, List<ComponentsInventoryList> componentsInventoryList)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    foreach (var register in componentsInventoryList)
                    {
                        using (SqlCommand cmd = new SqlCommand("dbo.spListInventoryUpdate", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@intFltTablFoli", SqlDbType.Int).Value = register.TableFolio;
                            cmd.Parameters.Add("@strMaintenanceDate", SqlDbType.NVarChar).Value = register.MaintenanceDate;
                            cmd.Parameters.Add("@strMaintenanceFolio", SqlDbType.NVarChar).Value = register.MaintenanceFolio;
                            cmd.Parameters.Add("@strSerialNumber", SqlDbType.NVarChar).Value = register.SerialNumber;
                            cmd.Parameters.Add("@strInstallationDate", SqlDbType.NVarChar).Value = register.InstallationDate;

                            var reader = _sqlResult.Put(clavePlaza, cmd, sql, "InventoryListUpdate");
                            if (reader.SqlResult == null)
                            {
                                return new Response
                                {
                                    Message = "Fail",
                                    Result = null
                                };
                            }
                        }
                    }
                    return new Response
                    {
                        Message = "Ok",
                        Result = componentsInventoryList
                    };
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ComponentDb: InventoryListUpdate", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }
    
        public Response GetComponentsData(string clavePlaza, int AgreementId )
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spComponentsDTCBox", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@AgreementId", SqlDbType.Int).Value = AgreementId;

                        var select = _sqlResult.GetList<ComponentsDTCBox>(clavePlaza, cmd, sql, "GetComponentsData");

                        List<ComponentsDTCBox> componentsDTCBoxes = (List<ComponentsDTCBox>)select.Result;
                        select.Result = null;

                        List<DtcBox> dtcBoxes = new List<DtcBox>();
                        var principales = componentsDTCBoxes.Where(x => x.VitalComponent == true).ToList();
                        var secundarios = componentsDTCBoxes.Where(x => x.VitalComponent == false).ToList();

                        //PRUEBA GIT
                        foreach (var principal in principales)
                        {

                            var dtcBox = new DtcBox
                            {
                                ComponentePrincipal = principal.Description
                            };

                            dtcBox.Secundarios = new List<ComponentDtcBoxPrincipal>
                            {
                                new ComponentDtcBoxPrincipal
                                {
                                    Description = principal.Description,
                                    AttachedId = principal.AttachedId,
                                    ComponentsRelationship = principal.ComponentsRelationship,
                                    VitalComponent = principal.VitalComponent,
                                    ComponentsRelationshipId = principal.ComponentsRelationship
                                }
                            };

                            List<ComponentsDTCBox> componentesProcesados = new List<ComponentsDTCBox>();

                            foreach (var secundario in secundarios)
                            {
                                int divisor = principal.ComponentsRelationship / 100;
                                decimal calculo = (secundario.ComponentsRelationship / 100);
                                if (Math.Floor(calculo) == divisor)
                                {
                                    string componenteSecundario = secundario.Description;
                                    dtcBox.Secundarios.Add(new ComponentDtcBoxPrincipal
                                    {
                                        Description = secundario.Description,
                                        AttachedId = secundario.AttachedId,
                                        ComponentsRelationship = secundario.ComponentsRelationship,
                                        VitalComponent = secundario.VitalComponent,
                                        ComponentsRelationshipId = principal.ComponentsRelationship
                                    });
                                    componentesProcesados.Add(secundario);
                                }
                                else
                                    break;
                            }
                            foreach (var item in componentesProcesados)
                                secundarios.Remove(item);
                            dtcBoxes.Add(dtcBox);
                        }
                        select.Result = dtcBoxes;
                        return select;
                    }


                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ComponentDb: GetComponentsData", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
            
        }

        public Response GetComponentsInventory(string clavePlaza, string squareId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand($"select Component Componente" +
        " from SquareInventory a join LanesCatalog b on (a.CapufeLaneNum = b.CapufeLaneNum and a.IdGare = b.IdGare)" +
        $" where b.SquareCatalogId = {squareId}" +
        " group by Component", sql);
                    return _sqlResult.GetList<ComponentsInventory>(clavePlaza, cmd, sql, "GetComponentsInventory");
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ComponentDb: GetComponentsInventory", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetComponentsInventoryUbication(string clavePlaza)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand($"select TypeUbicationId,Name Ubicacion from TypesUbication", sql);
                    return _sqlResult.GetList<ComponentsInventoryUbication>(clavePlaza, cmd, sql, "GetComponentsInventoryUbication");
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ComponentDb: GetComponentsInventoryUbication", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
        }

        public Response GetComponentsInventoryLane(string clavePlaza, string Component,string squareId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand($"select b.Lane " +
                        "from SquareInventory a " +
                        "join LanesCatalog b " +
                        "on (a.CapufeLaneNum = b.CapufeLaneNum and a.IdGare = b.IdGare) " +
                        $"where a.Component = '{Component}' and b.SquareCatalogId = '{squareId}' " +
                        "group by b.Lane", sql);
                    return _sqlResult.GetList<ComponentsInventoryLane>(clavePlaza, cmd, sql, "GetComponentsInventoryLane");
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ComponentDb: GetComponentsInventoryLane", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
            
        }

        public Response GetComponentsInventoryDescription(string clavePlaza, string Component, string Lane, string squareId)
        {
            try
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
                        $"where Component = '{Component}' and b.Lane = '{Lane}' and b.SquareCatalogId = '{squareId}'", sql);
                    return _sqlResult.GetList<ComponentsInventoryDescription>(clavePlaza, cmd, sql, "GetComponentsInventoryDescription");
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioDb: InsertComment", 1);
                return new Response { Message = $"Error: {ex.Message}", Result = null };
            }
            
        }


        public SqlResponse UpdateInventory(string clavePlaza, string Reference, int userId )
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.spUpdateRequestedFromInventory", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Reference", SqlDbType.NVarChar).Value = Reference;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        return _sqlResult.Post(clavePlaza, cmd, sql, "ComponentDb");
                    }
                }
            }
            catch (SqlException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ComponentDb: UpdateInventory", 1);
                return new SqlResponse { SqlMessage = ex.Message, SqlResult = null };
            }
        }
        #endregion
    }
}
