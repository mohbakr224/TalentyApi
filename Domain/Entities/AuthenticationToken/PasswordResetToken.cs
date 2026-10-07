using Domain.Common;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.AuthenticationTokenEntities
{
    public class PasswordResetToken 
    {
        public Guid UserId{ get; set; }
        public Guid Id { get; set; }

        public string TokenHash{ get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public DateTime? UsedAt { get; set; }

        public User User { get; set; } = null!;
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
        public bool IsActive => UsedAt.HasValue!;
    }
}
