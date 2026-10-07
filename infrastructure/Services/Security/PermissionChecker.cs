using Application.Interfaces.Security;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace infrastructure.Services.Security
{
    public class PermissionChecker : IPermissionChecker
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Application.Interfaces.Services.IPermissionService _permissionService;

        public PermissionChecker(
            IHttpContextAccessor httpContextAccessor,
            Application.Interfaces.Services.IPermissionService permissionService)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionService = permissionService;
        }

        public async Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission)
        {
            var principal = user ?? _httpContextAccessor.HttpContext?.User;
            if (principal == null || principal.Identity?.IsAuthenticated != true)
                return false;

            if (principal.IsInRole("ADMIN"))
                return true;

            var hasClaim = principal.Claims.Any(c => c.Type == "permission" && string.Equals(c.Value, permission, StringComparison.OrdinalIgnoreCase));
            if (hasClaim)
                return true;

            var uid = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(uid, out var userId))
            {
                var perms = await _permissionService.GetPermissionsForUserAsync(userId);
                if (perms.Any(p => string.Equals(p, permission, StringComparison.OrdinalIgnoreCase)))
                    return true;
            }

            return false;
        }
    }
}
