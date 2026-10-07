using Application.DTOs.Request;
using Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<NotificationResponse> SendNotificationAsync(SendNotificationRequest request, Guid senderId);

        Task<List<NotificationResponse>> GetUserNotificationsAsync(Guid userId, int limit, int offset, CancellationToken cancellationToken);

        Task<int> GetUnreadCountAsync(Guid userId);

        Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId);

        Task MarkAllAsReadAsync(Guid userId);
    }
}

