namespace ApiDTC.Models
{
    public class LoginToken
    {
        #region Properties
        public Login Login { get; set; }

        public UserToken UserToken { get; set; }
        #endregion
    }
}