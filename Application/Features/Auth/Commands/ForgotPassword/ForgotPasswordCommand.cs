using Common.Messaging;
using Common.Results;
using System;

namespace Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : Icommand<string>
    {
        public string Email { get; }

        public ForgotPasswordCommand(string email)
        {
            Email = email;
        }
    }
}
