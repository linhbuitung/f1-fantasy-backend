namespace F1Fantasy.Modules.NotificationModule.Services.Interfaces;

public interface INotificationService
{
    Task AddAndSendNotificationAsync(Dtos.Create.NotificationDto notification);

    Task AddAndSendNotificationForEachUserInForARaceAsync(int raceId);
    
    Task<Dtos.Get.NotificationDto> GetNotificationAsync(int notificationId);

    Task<List<Dtos.Get.NotificationDto>> GetAllUnreadNotificationsByUserIdAsync(int userId);
    
    Task<Dtos.Get.NotificationDto> MarkNotificationAsReadAsync(int userId, int notificationId);
    
    Task MarkAllNotificationsAsReadAsync(int userId);
}