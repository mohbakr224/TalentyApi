using Application.DTOs.Requests;
using Application.DTOs.Response;
using Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ResultT<AuthResponse>> RegisterAsync(RegisterRequest request);
    }
}
