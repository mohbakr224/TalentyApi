using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailVerificationAsync(string email,string verificationLink);
        Task SendPasswordResetAsync(string email,string resetLink);
    }
}
