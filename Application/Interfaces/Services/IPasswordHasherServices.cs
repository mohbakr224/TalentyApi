using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IPasswordHasherServices
    {
        string HashPassword(string password);
        bool VerifyPassword(string password , string passwordhash);
    }
}
