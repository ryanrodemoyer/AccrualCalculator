using Microsoft.AspNetCore.Authorization;

namespace AppName.Web.Transport
{
    public class NoOpRequirement : IAuthorizationRequirement
    {
        public NoOpRequirement()
        {
            
        }
    }
}