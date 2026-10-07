using Domain.Common;
using Domain.Enums;
using System.Collections.Generic;

namespace Domain.Entities.Identity
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
            = new List<UserRole>();

        public ICollection<RolePermission> RolePermissions { get; set; }
            = new List<RolePermission>();
    }
}