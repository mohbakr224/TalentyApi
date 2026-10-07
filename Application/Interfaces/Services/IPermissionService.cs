using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Application.Interfaces.Services
{
    public interface IPermissionService
    {
        Task<List<string>> GetPermissionsForUserAsync(Guid userId);
    }
}
