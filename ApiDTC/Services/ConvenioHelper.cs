using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Services
{
    public class ConvenioHelper
    {
        public List<ConvenioData> convenioDatas;
        public ConvenioHelper()
        {
            var lista = new List<string>() { "004", "069", "070" };

            convenioDatas = new List<ConvenioData>()
            {
                new ConvenioData
                {
                    Delegacion = 2,
                    ConvenioNuevo = true,
                    PlazasValidas = lista,
                    Agremment = "5500010044",
                    Email = "smosquedad@capufe.gob.mx",
                    Manager = "C. Saúl Mosqueda Delgado",
                    Cargo = "Subgerente de Operación",
                    Cordinacion = "Estado de México"
                }        
            };

        }
        public ConvenioData GetConvenioData(DataTable headerConvenio, DataTable dtcData)
        {
            DateTime dateValid = new DateTime(2021, 11, 28);
            DateTime sinisteDate = Convert.ToDateTime(dtcData.Rows[0]["SinisterDate"].ToString());
            int statusId = Convert.ToInt32(dtcData.Rows[0]["StatusId"].ToString());
            if(statusId != 4)
            {
                foreach(var item in convenioDatas)
                {
                    if (item.PlazasValidas.Contains(headerConvenio.Rows[0]["SquareCatalogId"].ToString()))
                    {
                        return item;
                    }
                }
            }
            return new ConvenioData
            {
                Delegacion = 2,
                ConvenioNuevo = false,
                Agremment = headerConvenio.Rows[0]["Agrement"].ToString(),
                PlazasValidas = null,
                Email = headerConvenio.Rows[0]["Mail"].ToString(),
                Manager = headerConvenio.Rows[0]["ManagerName"].ToString(),
                Cargo = headerConvenio.Rows[0]["Position"].ToString(),
                Cordinacion = headerConvenio.Rows[0]["RegionalCoordination"].ToString(),
            };
        }
   
    }
    public class ConvenioData
    {
        public int Delegacion { get; set; }
        public bool ConvenioNuevo { get; set; }
        public List<string> PlazasValidas { get; set; }
        public string Agremment { get; set; }
        public string Email { get; set; }
        public string Manager { get; set; }
        public string Cargo { get; set; }
        public string Cordinacion { get; set; }
    }
}
