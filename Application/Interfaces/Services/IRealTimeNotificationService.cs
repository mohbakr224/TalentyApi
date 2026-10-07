using Application.DTOs.Response;
using System;

namespace Application.Interfaces.Services
{
    public interface IRealTimeNotificationService
    {
        Task SendToUserAsync(Guid userId, NotificationResponse notification);
        Task SendToCompanyAsync(Guid companyId, NotificationResponse notification);
        Task SendToAllAsync(NotificationResponse notification);
    }
}