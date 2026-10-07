using Domain.Entities.Identity;
using Domain.Enums;
using System.Threading.Tasks;

namespace Application.Interfaces.Persistence
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(RoleType roleType);
    }
}
