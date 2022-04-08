namespace ApiDTC.Models
{
    public class SquaresCatalog
    {
        #region Properties
        public string SquareCatalogId { get; set; }
        
        public string SquareName { get; set; }
        
        public int DelegationId { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string ReferenceSquare { get; set; }

        #endregion
    }
}
