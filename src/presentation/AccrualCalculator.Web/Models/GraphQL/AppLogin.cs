using System;

namespace AppName.Web.Models
{
    public class AppLogin
    {
        public DateTime LoginTimestamp { get; set; }

        public AppLogin()
        {
            
        }

        public AppLogin(DateTime loginTimestamp)
        {
            LoginTimestamp = loginTimestamp;
        }
    }
}