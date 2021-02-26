
namespace ApiDTC.Models
{
    using System.Collections.Generic;
    
    public class LoginToken
    {
        #region Properties
        public List<Login> Login { get; set; }

        public UserToken UserToken { get; set; }
        #endregion
    }

    public class LoginTokenTrue
    {
        #region Properties
        public List<LoginTrue> Login { get; set; }

        public UserToken UserToken { get; set; }
        #endregion
    }
}