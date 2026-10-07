using Common.Messaging;
using Common.Results;
using System;

namespace Application.Features.Auth.Commands.Logout
{
    public class LogoutCommand : Icommand
    {
        public Guid UserId { get; }

        public LogoutCommand(Guid userId)
        {
            UserId = userId;
        }
    }
}
