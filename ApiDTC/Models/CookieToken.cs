namespace ApiDTC.Models
{
    public class CookieToken
    {
        #region Properties
        public Cookie Login { get; set; }

        public UserToken UserToken { get; set; }
        #endregion
    }
}