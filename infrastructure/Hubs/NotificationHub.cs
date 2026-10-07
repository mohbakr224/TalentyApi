using Microsoft.AspNetCore.SignalR;

namespace infrastructure.Hubs;

public class NotificationHub : Hub
{
    public async Task JoinUserGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
    }

    public async Task LeaveUserGroup(string userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
    }

    public async Task JoinCompanyGroup(string companyId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"company_{companyId}");
    }

    public async Task LeaveCompanyGroup(string companyId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"company_{companyId}");
    }
}