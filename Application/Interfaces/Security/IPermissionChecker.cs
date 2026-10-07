using System.Threading.Tasks;
using System.Security.Claims;

namespace Application.Interfaces.Security
{
    public interface IPermissionChecker
    {
        Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission);
    }
}
