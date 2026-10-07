using Domain.Common;
using System.Collections.Generic;

namespace Domain.Entities.Identity
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Navigation Properties
        public ICollection<RolePermission> RolePermissions { get; set; }
            = new List<RolePermission>();
    }
}