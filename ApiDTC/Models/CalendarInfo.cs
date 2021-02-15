namespace ApiDTC.Models
{
    using System.Collections.Generic;
    
    public class CalendarInfo
    {
        public CalendarHeader CalendarHeader { get; set; }

        public List<ActivitiesDescription> ActivitiesDescription { get; set; }
    }

}