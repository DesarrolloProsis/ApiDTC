namespace ApiDTC.Models
{
    using System;
    using System.Collections.Generic;

    public class LoginValido
    {
        #region Properties
        
        public LoginTrue LoginTrue { get; set; }

        public List<Login> LoginList { get; set; }
        
        #endregion
    }
}
