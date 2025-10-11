using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.NotificationModule.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<Notification> AddNotificationAsync(Notification notification);
    
    Task<List<FantasyLineup>> GetAllFantasyLineupsForARaceAsync(int raceId);
    
    
    Task<List<Notification>> GetAllNotificationsByUserIdAsync(int userId);

    Task<List<Notification>> GetAllUnreadNotificationsByUserIdAsTrackingAsync(int userId);

    Task<Notification?> GetNotificationAsTrackingAsync(int notificationId);

}