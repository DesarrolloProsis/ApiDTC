namespace ApiDTC.Models
{
    using System;
    public class DtcAlien
    {
        #region Properties
        public int UserId { get; set; }
        
        public string ReferenceNumber { get; set; }

        public DateTime DateStamp { get; set; }
        #endregion
    }
}