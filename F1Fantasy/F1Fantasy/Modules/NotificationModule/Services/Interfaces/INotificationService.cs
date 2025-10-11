namespace F1Fantasy.Modules.NotificationModule.Services.Interfaces;

public interface INotificationService
{
    Task<Dtos.Get.NotificationDto> AddNotificationAsync(Dtos.Create.NotificationDto notification);

    Task AddNotificationForEachUserInForARaceAsync(int raceId);
}