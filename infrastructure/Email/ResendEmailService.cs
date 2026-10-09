using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using Resend;

namespace infrastructure.Email
{
    public sealed class ResendEmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly ILogger<ResendEmailService> _logger;
        private readonly string _from;

        public ResendEmailService(
            IConfiguration configuration,
            ILogger<ResendEmailService> logger)
        {
            var apiKey = configuration["Email:ResendApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "Email:ResendApiKey is not configured. Set it with user secrets or the Email__ResendApiKey environment variable.");
            }

            _resend = ResendClient.Create(apiKey);
            _logger = logger;
            _from = configuration["Email:Username"];
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
                var response = await _resend.EmailSendAsync(new EmailMessage
                {
                    From = _from,
                    To = toEmail,
                    Subject = subject,
                    HtmlBody = htmlBody
                });

                _logger.LogInformation(
                    "Email sent to {Email} through Resend. Email ID: {EmailId}",
                    toEmail,
                    response.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email} through Resend", toEmail);
                throw new InvalidOperationException(
                    $"Failed to send email to {toEmail} through Resend.", ex);
            }
        }
    }
}
