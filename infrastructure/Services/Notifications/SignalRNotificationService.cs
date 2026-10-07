using Application.DTOs.Response;
using Application.Interfaces.Services;
using infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace infrastructure.Services.Notifications
{
    public class SignalRNotificationService : IRealTimeNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(Guid userId, NotificationResponse notification)
        {
            await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", notification);
        }

        public async Task SendToCompanyAsync(Guid companyId, NotificationResponse notification)
        {
            await _hubContext.Clients.Group($"company_{companyId}").SendAsync("ReceiveNotification", notification);
        }

        public async Task SendToAllAsync(NotificationResponse notification)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
        }
    }
}