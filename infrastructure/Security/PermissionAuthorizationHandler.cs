using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;

namespace infrastructure.Security
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }
        public PermissionRequirement(string permission) => Permission = permission;
    }

    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                return Task.CompletedTask;
            }

            var has = context.User.Claims.Any(c => c.Type == "permission" && c.Value == requirement.Permission);
            if (has)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
