using Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommand:Icommand
    {
        public string Token { get;}
        public VerifyEmailCommand(string token)
        {
            Token = token;
        }
    }
}
