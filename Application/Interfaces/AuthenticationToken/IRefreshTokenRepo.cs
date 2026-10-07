using Domain.Entities.AuthenticationTokenEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.AuthenticationToken
{
    public interface IRefreshTokenRepo
    {
        Task AddRefreshToken(
            RefreshToken token,
            CancellationToken cancellationToken);

        Task UpdateRefreshToken(
            RefreshToken token,
            CancellationToken cancellationToken);

        Task<List<RefreshToken>> GetRefreshTokensByUserId(
            Guid userId,
            CancellationToken cancellationToken);
    }
}
