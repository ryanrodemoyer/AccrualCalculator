using System;

namespace AppName.Web.Models
{
    public class Healthcheck
    {
        public string Version { get; }

        public DateTime ServerTimestamp { get; }

        public Healthcheck(string version, DateTime serverTimestamp)
        {
            Version = version;
            ServerTimestamp = serverTimestamp;
        }
    }
}