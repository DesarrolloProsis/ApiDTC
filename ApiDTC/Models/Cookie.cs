namespace ApiDTC.Models
{
    public class Cookie
    {
        #region Properties
        public int UserId { get; set; }
        
        public string SquareCatalogId { get; set; }
        
        public int RollId { get; set; }

        public string SquareName { get; set; }

        public string ReferenceSquare { get; set; }

        public int AdminSquareId { get; set; }

        public string PlazaAdministrador { get; set; }

        public bool StatusAdmin { get; set; }
        #endregion
    }
}