using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AppName.Web.Transport
{
    public class NoOpHandler : AuthorizationHandler<NoOpRequirement>
    {
        public NoOpHandler()
        {
            
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NoOpRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated && context.User.IsInRole("Access.Api"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}