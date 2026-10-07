using Application.DTOs.Requests;
using Application.DTOs.Response;
using Common.Messaging;

namespace Application.Features.Auth.Commands.Login
{
    public class LoginCommand : Icommand<AuthResponse>
    {
        public LoginRequest request { get; set; }
        public string Password { get; set; } = string.Empty;

        public LoginCommand(LoginRequest request)
        {
            this.request = request;
        }
    }
}