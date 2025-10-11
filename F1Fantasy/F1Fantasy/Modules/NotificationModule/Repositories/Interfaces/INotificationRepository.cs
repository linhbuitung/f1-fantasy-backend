using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.NotificationModule.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<Notification> AddNotificationAsync(Notification notification);
    
    Task<List<FantasyLineup>> GetAllFantasyLineupsForARaceAsync(int raceId);
}