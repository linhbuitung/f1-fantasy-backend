using F1Fantasy.Core.Common;
using F1Fantasy.Modules.AskAiModule.Services.Interfaces;
using F1Fantasy.Modules.NotificationModule.Dtos.Create;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace F1Fantasy.Core.Middlewares;

public class UserInteractionTrackMiddleware(RequestDelegate next, ILogger<UserInteractionTrackMiddleware> logger, IServiceProvider serviceProvider, IConfiguration configuration )
{
    
    public async Task InvokeAsync(HttpContext context)
    {
        var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        var askAiService = context.RequestServices.GetRequiredService<IAskAIService>();

        if (IsUserAuthenticated(context))
        {
            var userId = userManager.GetUserId(context.User);
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await userManager.FindByIdAsync(userId);
                if (ShouldUpdateUserActivity(user))
                {
                    await UpdateUserActivity(user, askAiService, context);
                    await userManager.UpdateAsync(user);
                }
            }
        }

        await next(context);
    }

    private bool IsUserAuthenticated(HttpContext context)
    {
        return context.User.Identity?.IsAuthenticated == true;
    }

    private bool ShouldUpdateUserActivity(ApplicationUser? user)
    {
        if (user == null) return false;
        return user.LastActiveAt == null || user.LastActiveAt < DateTime.UtcNow.AddMinutes(-5);
    }

    private async Task UpdateUserActivity(ApplicationUser user, IAskAIService askAIService, HttpContext context)
    {
        var hub = context.RequestServices.GetRequiredService<Microsoft.AspNetCore.SignalR.IHubContext<F1Fantasy.Modules.NotificationModule.NotificationHub>>();
        var notificationService = context.RequestServices.GetRequiredService<F1Fantasy.Modules.NotificationModule.Services.Interfaces.INotificationService>();
        var consecutiveActiveDaysPerCreditAdded = configuration.GetSection("CoreGameplaySettings:AskAiSettings:ConsecutiveActiveDaysPerCreditAdded").Get<int>();

        var lastActive = user.LastActiveAt;
        var now = DateTime.UtcNow;

        if (lastActive.HasValue)
        {
            if (lastActive.Value.Date == now.AddDays(-1).Date)
            {
                user.ConsecutiveActiveDays += 1;
            }
            else if (lastActive.Value.Date < now.AddDays(-1).Date)
            {
                user.ConsecutiveActiveDays = 1;
            }
        }
        else
        {
            user.ConsecutiveActiveDays = 1;
        }

        if (user.ConsecutiveActiveDays < consecutiveActiveDaysPerCreditAdded)
        {
            await notificationService.AddAndSendNotificationAsync(
                new NotificationDto
                {
                    UserId = user.Id,
                    Header = "AI Progress",
                    Content = $"You've logged in for {user.ConsecutiveActiveDays}, {consecutiveActiveDaysPerCreditAdded - user.ConsecutiveActiveDays} more days to earn an extra AI credit!",
                });
        }
        else
        {
            await askAIService.AddAskAiCreditAsync(userId: user.Id);
            user.ConsecutiveActiveDays = 0;
            await notificationService.AddAndSendNotificationAsync(
                new NotificationDto
                {
                    UserId = user.Id,
                    Header = "AI Reward",
                    Content = "You've earned an extra AI credit for your activity! Keep it up!",
                });
        }
        user.LastActiveAt = now;
    }
}
