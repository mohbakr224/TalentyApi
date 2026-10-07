using Application.Interfaces.Services;
using Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace infrastructure.Services.Security
{
    public class PermissionService : IPermissionService
    {
        private readonly Application.Interfaces.Persistence.IUserRepository _userRepository;
        private readonly infrastructure.Persistence.AppDBcontext _dbContext;

        public PermissionService(Application.Interfaces.Persistence.IUserRepository userRepository, infrastructure.Persistence.AppDBcontext dbContext)
        {
            _userRepository = userRepository;
            _dbContext = dbContext;
        }

        public async Task<List<string>> GetPermissionsForUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return new List<string>();

            var roleIds = user.UserRoles?.Select(ur => ur.RoleId).ToList() ?? new List<Guid>();
            if (!roleIds.Any())
                return new List<string>();

            var perms = await _dbContext.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync();

            return perms;
        }
    }
}
