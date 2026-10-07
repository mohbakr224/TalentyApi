using Application.Interfaces.Persistence;
using Domain.Entities.Notifications;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace infrastructure.Repository.Notifications
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDBcontext _context;

        public NotificationRepository(AppDBcontext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications
                .Include(n => n.Sender)
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task UpdateAsync(Notification notification)
        {
            notification.UpdatedAt = DateTime.UtcNow;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetByUserIdAsync(Guid userId, int limit, int offset, CancellationToken cancellationToken)
        {
            return await _context.Notifications
                .Include(n => n.Sender)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .Skip(offset)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var unread = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            var now = DateTime.UtcNow;
            foreach (var n in unread)
            {
                n.IsRead = true;
                n.ReadAt = now;
                n.UpdatedAt = now;
            }

            await _context.SaveChangesAsync();
        }
    }
}

