using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Security
{
    public interface IJwtService
    {
        string GenerateAccessToken(Guid userid, string email, IEnumerable<string> roles, IEnumerable<string>? permissions = null);
        string GenerateRefreshToken();
        string HashRefreshToken(string token);

    }
}
