using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ApiDTC.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            TypeInfo t = typeof(Components).GetTypeInfo();
            IEnumerable<PropertyInfo> pList = t.DeclaredProperties;

            StringBuilder sb = new StringBuilder();
           var propiedades = new List<string>();
            foreach (PropertyInfo p in pList)
            {
                propiedades.Add(p.DeclaringType.Name + ": " + p.Name);
            }
            return propiedades;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
