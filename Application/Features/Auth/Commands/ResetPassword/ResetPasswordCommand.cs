using Common.Messaging;

namespace Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommand : Icommand
    {
        public string Token { get; }

        public string NewPassword { get; }

        public ResetPasswordCommand(string token, string newPassword)
        {
            Token = token;
            NewPassword = newPassword;
        }
    }
}
