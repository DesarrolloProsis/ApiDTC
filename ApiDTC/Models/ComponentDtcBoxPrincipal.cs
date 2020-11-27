namespace ApiDTC.Models
{
    public class ComponentDtcBoxPrincipal
    {
        public string Description { get; set; }

        public int AttachedId { get; set; }

        public int ComponentsRelationshipId { get; set; }

        public int ComponentsRelationship { get; set; }

        public bool VitalComponent { get; set; }
    }
}
