using Domain.Entities.AuthenticationToken;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.AuthenticationToken
{
    public interface IAccessTokenRepo
    {
        Task AddAccessToken(AccessToken token, CancellationToken cancellationToken);

        Task UpdateAccessToken(AccessToken token, CancellationToken cancellationToken);

        Task<List<AccessToken>> GetAccessTokensByUserId(Guid userId, CancellationToken cancellationToken);
    }
}
