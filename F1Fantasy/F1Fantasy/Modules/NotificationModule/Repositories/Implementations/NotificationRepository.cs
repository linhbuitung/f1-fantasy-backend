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
}