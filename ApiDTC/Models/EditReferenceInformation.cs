namespace ApiDTC.Models
{
    using System.Collections.Generic;

    public class EditReferenceInformation
    {
        public List<DtcSerialNumbers> SerialNumbers { get; set; }

        public List<DtcItems> Items { get; set; }
    }
}
