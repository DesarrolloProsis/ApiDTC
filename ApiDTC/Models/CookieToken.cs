
namespace ApiDTC.Models
{
    using System.Collections.Generic;
    public class CookieToken
    {
        #region Properties
        public LoginTrue LoginTrue { get; set; }
        
        public List<Cookie> Cookie { get; set; }

        public UserToken UserToken { get; set; }
        #endregion
    }
}