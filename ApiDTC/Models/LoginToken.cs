namespace ApiDTC.Models
{
    public class LoginToken
    {
        #region Properties
        public Login Login { get; set; }

        public UserToken UserToken { get; set; }
        #endregion
    }

    public class LoginTokenTrue
    {
        #region Properties
        public LoginTrue Login { get; set; }

        public UserToken UserToken { get; set; }
        #endregion
    }
}