﻿namespace ApiDTC.Models
{
    using System;

    public class Login
    {
        #region Properties
        public int UserId { get; set; }
        
        public int AgremmentInfoId { get; set; }
        
        public string Nombre { get; set; }
        
        public string Plaza { get; set; }
        
        public string Agrement { get; set; }
        
        public string ManagerName { get; set; }
        
        public string Position { get; set; }
        
        public string Mail { get; set; }
        
        public DateTime AgremmentDate { get; set; }
        
        public string DelegationName { get; set; }
        
        public string RegionalCoordination { get; set; }
        #endregion
    }
}
