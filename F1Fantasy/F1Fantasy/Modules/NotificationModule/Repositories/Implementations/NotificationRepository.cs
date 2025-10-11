using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.NotificationModule.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.NotificationModule.Repositories.Implementations;

public class NotificationRepository(WooF1Context context) : INotificationRepository
{
    public async Task<Notification> AddNotificationAsync(Notification notification)
    {
        context.Notifications.Add(notification);
        await context.SaveChangesAsync();
        return notification;
    }

    public async Task<List<FantasyLineup>> GetAllFantasyLineupsForARaceAsync(int raceId)
    {
        return await context.FantasyLineups
            .Where(l => l.RaceId == raceId)
            .Include(fl => fl.FantasyLineupConstructors)
            .ThenInclude(fl => fl.Constructor)
            .Include(fl => fl.FantasyLineupDrivers)
            .ThenInclude(fl => fl.Driver)
            .ToListAsync();
            
    }
    
    public async Task<List<Notification>> GetAllNotificationsByUserIdAsync(int userId)
    {
        return await context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Notification>> GetAllUnreadNotificationsByUserIdAsTrackingAsync(int userId)
    {
        return await context.Notifications
            .AsTracking()
            .Where(n => n.UserId == userId && n.ReadAt == null)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notification?> GetNotificationAsTrackingAsync(int notificationId)
    {
        return await context.Notifications.AsTracking().FirstOrDefaultAsync(n => n.Id == notificationId);

    }
}