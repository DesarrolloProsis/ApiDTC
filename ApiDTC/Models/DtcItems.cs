namespace ApiDTC.Models
{
    public class DtcItems
    {
        public int item { get; set; }

        public string Name { get; set; }

        //public string Marca { get; set; }
        public int AttachedId { get; set; }

        public int Relationship { get; set; }

        public int MainRelationship { get; set; }

        public bool VitalComponent { get; set; }
    }
}
