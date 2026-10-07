using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Domain.Entities.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace infrastructure.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IRealTimeNotificationService _realTimeNotificationService;

        public NotificationService(
            INotificationRepository notificationRepository,
            IRealTimeNotificationService realTimeNotificationService)
        {
            _notificationRepository = notificationRepository;
            _realTimeNotificationService = realTimeNotificationService;
        }

        public async Task<NotificationResponse> SendNotificationAsync(SendNotificationRequest request, Guid senderId)
        {
            var entity = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                SenderId = senderId,
                CompanyId = request.CompanyId,
                Title = request.Title,
                Message = request.Message,
                Type = string.IsNullOrWhiteSpace(request.Type) ? "RECRUITMENT" : request.Type,
                RelatedEntityId = request.RelatedEntityId,
                IsRead = false,
                CreatedDate = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(entity);

            var loaded = await _notificationRepository.GetByIdAsync(entity.Id);
            var response = MapToResponse(loaded ?? entity);

            await _realTimeNotificationService.SendToUserAsync(request.UserId, response);

            if (request.CompanyId.HasValue)
            {
                await _realTimeNotificationService.SendToCompanyAsync(request.CompanyId.Value, response);
            }

            return response;
        }

        public async Task<List<NotificationResponse>> GetUserNotificationsAsync(Guid userId, int limit, int offset, CancellationToken cancellationToken)
        {
            var list = await _notificationRepository.GetByUserIdAsync(userId, limit, offset, cancellationToken);
            return list.Select(MapToResponse).ToList();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _notificationRepository.GetUnreadCountAsync(userId);
        }

        public async Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            var notif = await _notificationRepository.GetByIdAsync(notificationId);
            if (notif == null || notif.UserId != userId) return false;

            notif.IsRead = true;
            notif.ReadAt = DateTime.UtcNow;
            await _notificationRepository.UpdateAsync(notif);
            return true;
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await _notificationRepository.MarkAllAsReadAsync(userId);
        }

        private static NotificationResponse MapToResponse(Notification n)
        {
            var senderName = n.Sender != null ? $"{n.Sender.FirstName} {n.Sender.LastName}".Trim() : null;

            return new NotificationResponse
            {
                Id = n.Id,
                UserId = n.UserId,
                SenderId = n.SenderId,
                SenderName = string.IsNullOrEmpty(senderName) ? null : senderName,
                CompanyId = n.CompanyId,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                RelatedEntityId = n.RelatedEntityId,
                IsRead = n.IsRead,
                ReadAt = n.ReadAt,
                CreatedDate = n.CreatedDate
            };
        }
    }
}

