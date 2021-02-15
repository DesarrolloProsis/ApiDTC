namespace ApiDTC.Models
{
    public class ActivitiesDescription
    {
        public string ActivityDescription { get; set; }

        public string ComponentDescription { get; set; }

        public string JobDescription { get; set; }

        public string Name { get; set; }

        public string StatusDescription { get; set; }

        public int JobStatusId { get; set; }

        public int ComponentsJobId { get; set; }
    }
}