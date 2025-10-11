namespace F1Fantasy.Modules.NotificationModule.Services.Interfaces;

public interface INotificationService
{
    Task AddAndSendNotificationAsync(Dtos.Create.NotificationDto notification);

    Task AddNotificationForEachUserInForARaceAsync(int raceId);
}