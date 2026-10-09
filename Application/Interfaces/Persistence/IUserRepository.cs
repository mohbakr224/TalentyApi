using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Persistence
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User>GetUserByIdAsync(Guid id);
        Task AddAsync(User user);
        Task DeleteAsync(User user);
        Task UpdateAsync(User user);
        Task SaveChangesAsync();
    }
}
