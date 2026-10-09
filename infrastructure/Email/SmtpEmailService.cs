using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using System.Text.Encodings.Web;

namespace infrastructure.Email
{
    public sealed class SmtpEmailService : IEmailService
    {
        private readonly ILogger<SmtpEmailService> _logger;
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly bool _enableSsl;

        public SmtpEmailService(
            IConfiguration configuration,
            ILogger<SmtpEmailService> logger)
        {
            _logger = logger;
            _host = configuration["Email:SmtpHost"] ?? throw new InvalidOperationException("Email:SmtpHost is not configured.");
            _port = configuration.GetValue<int>("Email:SmtpPort", 587);
            _username = configuration["Email:Username"] ?? throw new InvalidOperationException("Email:Username is not configured.");
            _password = configuration["Email:Password"] ?? throw new InvalidOperationException("Email:Password is not configured.");
            _enableSsl = configuration.GetValue<bool>("Email:EnableSsl", true);
        }

        public Task SendEmailVerificationAsync(string email, string verificationLink)
        {
            var safeLink = HtmlEncoder.Default.Encode(verificationLink);
            var body = $"""
                       <p>Welcome to CareerHub!</p>
                       <p>Please verify your email address by clicking the link below:</p>
                       <p><a href="{safeLink}">Verify your email</a></p>
                       <p>This link will expire in 24 hours.</p>
                       """;

            return SendEmailAsync(email, "Verify your CareerHub email", body);
        }

        public Task SendPasswordResetAsync(string email, string resetLink)
        {
            var safeLink = HtmlEncoder.Default.Encode(resetLink);
            var body = $"""
                       <p>Use the following link to reset your CareerHub password:</p>
                       <p><a href="{safeLink}">Reset your password</a></p>
                       <p>This link will expire in 1 hour.</p>
                       """;

            return SendEmailAsync(email, "Reset your CareerHub password", body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                using var client = new SmtpClient(_host, _port)
                {
                    EnableSsl = _enableSsl,
                    Credentials = new NetworkCredential(_username, _password)

                };

                using var message = new MailMessage
                {
                    From = new MailAddress(_username),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                message.To.Add(toEmail);

                await client.SendMailAsync(message);

                _logger.LogInformation("Email sent to {Email} through SMTP", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email} through SMTP", toEmail);
                throw new InvalidOperationException($"Failed to send email to {toEmail} through SMTP.", ex);
            }
        }
    }
}