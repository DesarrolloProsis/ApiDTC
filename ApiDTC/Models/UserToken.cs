namespace ApiDTC.Models
{
    using System;
    
    public class UserToken
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}