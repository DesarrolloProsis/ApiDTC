namespace ApiDTC.Models
{
    public class Response
    {
        #region Properties
        public string Message { get; set; }

        public object Result { get; set; }

        public int Rows { get; set; }
        #endregion
    }
}