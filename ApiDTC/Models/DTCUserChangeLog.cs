namespace ApiDTC.Models.Logs
{
    public class DTCUserChangeLog
    {
        public int UserId { get; set; }
        public string ReferenceNumberDTC { get; set; }
        public string ReferenceNumberDiagnostic { get; set; }
        public string Comment { get; set; }
    }
}