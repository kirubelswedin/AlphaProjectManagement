using Microsoft.AspNetCore.SignalR;

namespace Hubs;

public class NotificationHub : Hub
{
    // Real-time notification hub using SignalR, send notifications to all connected clients.
    public async Task SendNotification(object notification)
    {
        await Clients.All.SendAsync("ReceiveNotification", notification);
    }
}
