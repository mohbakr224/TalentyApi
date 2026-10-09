using Application.Interfaces.Persistence;
using Domain.Entities.Identity;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repository.Identity
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDBcontext _dbContext;

        public UserRepository(AppDBcontext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
        }

        public Task DeleteAsync(User user)
        {
            _dbContext.Users.Remove(user);
            return Task.CompletedTask;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbContext.Users
                .Include(user => user.UserRoles)
                    .ThenInclude(userRole => userRole.Role)
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public Task UpdateAsync(User user)
        {
            _dbContext.Users.Update(user);
            return Task.CompletedTask;
        }


        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _dbContext.Users
                .Include(user => user.UserRoles)
                    .ThenInclude(userRole => userRole.Role)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
