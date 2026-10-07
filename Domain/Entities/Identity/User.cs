using Domain.Common;
using Domain.Entities.AuthenticationToken;
using Domain.Entities.AuthenticationTokenEntities;
using Domain.Entities.Candidates;
using Domain.Entities.Companies;
using System;
using System.Collections.Generic;

namespace Domain.Entities.Identity
{
    public class User : BaseEntity, ISoftDeletable
    {
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool EmailVerified { get; set; } = false;

        public bool IsLocked { get; set; } = false;

        public DateTime? LastLoginAt { get; set; }

        // Soft Delete
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
        public ICollection<RefreshToken>refreshTokens = new List<RefreshToken>();
        public ICollection<AccessToken> AccessTokens { get; set; } = new List<AccessToken>();
        public ICollection<PasswordResetToken> passwordResetTokens = new List<PasswordResetToken>();
       
        public ICollection<EmailVerificationTokens> emailVerificationTokens = new List<EmailVerificationTokens>();

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public CandidateProfile? CandidateProfile { get; set; }

        public ICollection<CompanyMember> CompanyMemberships { get; set; }
            = new List<CompanyMember>();
    }
}