using Domain.Entities.Notifications;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Persistence
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<Notification?> GetByIdAsync(Guid id);
        Task UpdateAsync(Notification notification);
        Task<List<Notification>> GetByUserIdAsync(Guid userId, int limit, int offset, CancellationToken cancellationToken);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task MarkAllAsReadAsync(Guid userId);
    }
}

