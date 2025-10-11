using F1Fantasy.Modules.NotificationModule.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace F1Fantasy.Modules.NotificationModule.Controllers;

[ApiController]
[Route("notification")]
public class NotificationController(IHubContext<NotificationHub> hubContext, INotificationService notificationService) : ControllerBase
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

        await notificationService.AddNotificationForEachUserInForARaceAsync(raceId);
        return Ok();
        // TO DO
    }

}