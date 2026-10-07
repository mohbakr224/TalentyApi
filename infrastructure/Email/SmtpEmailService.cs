using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace infrastructure.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailService(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailVerificationAsync(
            string email,
            string verificationLink)
        {
            var smtpHost =
                _configuration["Email:SmtpHost"];

            var smtpPort =
                int.Parse(_configuration["Email:SmtpPort"]!);

            var username =
                _configuration["Email:Username"];

            var password =
                _configuration["Email:Password"];

            using var client =
                new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(
                        username,
                        password),

                    EnableSsl = true
                };

            using var message = new MailMessage
            {
                From = new MailAddress(username!),

                Subject = "Verify your CareerHub email",

                Body = $"""
                       Welcome to CareerHub!

                       Please verify your email address
                       by clicking the link below:

                       {verificationLink}

                       This link will expire in 24 hours.
                       """,

                IsBodyHtml = false
            };

            message.To.Add(email);

            await client.SendMailAsync(message);
        }

        public async Task SendPasswordResetAsync(string email, string resetLink)
        {
            var smtpHost =
                _configuration["Email:SmtpHost"];

            var smtpPort =
                int.Parse(_configuration["Email:SmtpPort"]!);

            var username =
                _configuration["Email:Username"];

            var password =
                _configuration["Email:Password"];

            using var client =
                new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(
                        username,
                        password),

                    EnableSsl = true
                };

            using var message = new MailMessage
            {
                From = new MailAddress(username!),

                Subject = "Reset your CareerHub password",

                Body = $"Use the following link to reset your password: {resetLink}\nThis link will expire in 1 hour.",

                IsBodyHtml = false
            };

            message.To.Add(email);

            await client.SendMailAsync(message);
        }
    }
}