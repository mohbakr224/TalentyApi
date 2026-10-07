using Domain.Common;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.AuthenticationToken
{
    public class AccessToken 
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpireAt { get; set; }
        public DateTime? RevokeAt { get; set; }
        public bool IsExpired => DateTime.UtcNow > ExpireAt;
        public User User { get; set; }

    }
}
