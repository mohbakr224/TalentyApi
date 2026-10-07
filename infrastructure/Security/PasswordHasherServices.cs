using System;
using System.Collections.Generic;
using System.Text;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
namespace infrastructure.Security
{
    public class PasswordHasherServices : IPasswordHasherServices
    {
        private readonly PasswordHasher<object> _passwordHasher;

        public PasswordHasherServices()
        {
            _passwordHasher = new PasswordHasher<object>();
        }
        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null!, password);
        }

        public bool VerifyPassword(
               string password,
               string passwordHash)
        {
            var result = _passwordHasher.VerifyHashedPassword(
                null!,
                passwordHash,
                password
            );

            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
