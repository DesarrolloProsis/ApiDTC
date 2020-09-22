using System.Collections.Generic;

namespace ApiDTC.Models
{
    public class ResultEditReferenceOpen
    {
        public List<EditRequestedComponent> RequestedComponents { get; set; }

        public List<ProposedComponentsOpen> ProposedComponents { get; set; }
    }
}
