using Application.Interfaces.Persistence;
using Domain.Entities.Identity;
using Domain.Enums;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repository.Identity
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDBcontext _dbContext;

        public RoleRepository(AppDBcontext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Role?> GetByNameAsync(RoleType roleType)
        {
            return await _dbContext.Roles
                .FirstOrDefaultAsync(r => r.Name == roleType.ToString());
        }
    }
}