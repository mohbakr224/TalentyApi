using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Persistence
{
    public interface IUserRoleRepository
    {
        Task AddASync(UserRole userRole);
    }
}
