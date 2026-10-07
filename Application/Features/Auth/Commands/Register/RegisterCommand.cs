using Application.DTOs.Requests;
using Application.DTOs.Response;
using Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Commands.Register
{
    public class RegisterCommand:Icommand<AuthResponse>
    {
        public RegisterCommand(RegisterRequest request)
        {
            Request = request;
        }

        public RegisterRequest Request { get; set; }
    }
}
