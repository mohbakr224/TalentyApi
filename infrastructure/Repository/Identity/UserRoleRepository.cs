using Application.Interfaces.Persistence;
using Domain.Entities.Identity;
using infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace infrastructure.Repository.Identity
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDBcontext _appDBContext;

        public UserRoleRepository(AppDBcontext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        public async Task AddASync(UserRole userRole)
        {
            _appDBContext.UserRoles.Add(userRole);
            await _appDBContext.SaveChangesAsync();
        }
    }
}
