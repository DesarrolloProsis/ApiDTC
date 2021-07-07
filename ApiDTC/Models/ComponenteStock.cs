namespace ApiDTC.Models
{
    public class ComponenteStock
    {
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Unity { get; set; }
        public bool VitalComponent { get; set; }
        public int ComponentsRelationship { get; set; }
        public int AttachedId { get; set; }
    }
}
