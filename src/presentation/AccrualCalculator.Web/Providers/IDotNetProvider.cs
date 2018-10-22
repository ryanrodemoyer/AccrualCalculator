using System;

namespace AppName.Web.Providers
{
    public interface IDotNetProvider
    {
        DateTime DateTimeNow { get; }
        
        Guid NewGuid { get; }
    }

    public class DefaultDotNetProvider : IDotNetProvider
    {
        public DateTime DateTimeNow => DateTime.Now;

        public Guid NewGuid => Guid.NewGuid();
    }
}