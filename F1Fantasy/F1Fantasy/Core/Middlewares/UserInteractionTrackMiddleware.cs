using F1Fantasy.Core.Common;
using F1Fantasy.Modules.AskAiModule.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

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
                    await UpdateUserActivity(user, askAiService);
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

    private async Task UpdateUserActivity(ApplicationUser user, IAskAIService askAIService)
    {
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

        var consecutiveActiveDaysPerCreditAdded = configuration.GetValue<int?>("CoreGameplaySettings:AskAiSettings:ConsecutiveActiveDaysPerCreditAdded") ?? 7;
        if (user.ConsecutiveActiveDays >= consecutiveActiveDaysPerCreditAdded)
        {
            await askAIService.AddAskAiCreditAsync(userId: user.Id);
        }
        user.LastActiveAt = now;
    }
}
