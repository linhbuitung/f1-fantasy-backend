using F1Fantasy.Core.Policies;
using F1Fantasy.Modules.NotificationModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace F1Fantasy.Modules.NotificationModule.Controllers;

[ApiController]
[Route("notification")]
public class NotificationController(
    IHubContext<NotificationHub> hubContext,
    IAuthorizationService authorizationService,
    INotificationService notificationService) : ControllerBase
{
    public async Task BroadcastNotification(Dtos.Get.NotificationDto notification)
    { 
       await hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
    }
    
    public async Task SendNotificationToUser (string userConnectionId, string message)
    {
        await hubContext.Clients.Client(userConnectionId).SendAsync("ReceiveNotification", message);
    }
    
    [HttpPost("worker/race-calculated/{raceId:int}")]
    public async Task<IActionResult> SendRaceCalculatedNotification(int raceId)
    {
        var apiKey = Request.Headers["Worker-Api-Key"].FirstOrDefault();
        var expectedApiKey = Environment.GetEnvironmentVariable("WORKER_API_KEY");
        if (apiKey is null ||expectedApiKey is null || apiKey != expectedApiKey) return BadRequest();

        await notificationService.AddAndSendNotificationForEachUserInForARaceAsync(raceId);
        return Ok();
        // TO DO
    }

    [HttpGet("{notificationId:int}")]
    public async Task<IActionResult> GetNotification(int notificationId)
    {
        var notifications = await notificationService.GetNotificationAsync(notificationId);
        return Ok(notifications);
    }
    
    [HttpGet("user/{userId:int}/unread")]
    public async Task<IActionResult> GetUnreadNotifications(int userId)
    {
        var notifications = await notificationService.GetAllUnreadNotificationsByUserIdAsync(userId);
        return Ok(notifications);
    }
    
    [HttpPatch("/user/{userId:int}/notification/{notificationId:int}/read")]
    public async Task<IActionResult> MarkNotificationAsRead(int userId, int notificationId)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        var notification = notificationService.MarkNotificationAsReadAsync(userId, notificationId);
        return Ok(notification);
    }

    [HttpPatch("/user/{userId:int}/notifications/mark-all-read")]
    public async Task<IActionResult> MarkAllNotificationsAsRead(int userId)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        await notificationService.MarkAllNotificationsAsReadAsync(userId);
        return Ok();
    }

}