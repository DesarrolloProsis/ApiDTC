namespace ApiDTC.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PdfHeader
    {
        #region Properties
        public string Nombre { get; set; }  
        
        public string Plaza { get; set; }

        public string Agrement { get; set; }

        public string ManagerName { get; set; }
        
        public string Position { get; set; }

        public string Mail { get; set; }

        public DateTime AgremmentDate { get; set; }
        
        public string DelegationName { get; set; }

        [Key]
        public int UserId { get; set; }

        [Key]
        public int AgremmentInfoId { get; set; }
        
        public string RegionalCoordination { get; set; }
        #endregion
    }
}