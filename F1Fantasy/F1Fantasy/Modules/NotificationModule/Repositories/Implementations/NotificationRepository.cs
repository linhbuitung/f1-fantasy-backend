using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.NotificationModule.Repositories.Interfaces;

namespace F1Fantasy.Modules.NotificationModule.Repositories.Implementations;

public class NotificationRepository(WooF1Context context) : INotificationRepository
{
    public async Task<Notification> AddNotificationAsync(Notification notification)
    {
        context.Notifications.Add(notification);
        await context.SaveChangesAsync();
        return notification;
    }
}