using Microsoft.AspNetCore.SignalR;

namespace F1Fantasy.Modules.NotificationModule;

public class NotificationHub : Hub
{
    // These methods not working 
    
    // send single notification to specific user
    public Task SendNotificationToUser(string userConnectionId, Dtos.Get.NotificationDto notification) =>
        Clients.Client(userConnectionId).SendAsync("ReceiveNotification", notification);

    // send broadcast
    public Task BroadcastNotification(Dtos.Get.NotificationDto notification) =>
        Clients.All.SendAsync("ReceiveNotification", notification);
}