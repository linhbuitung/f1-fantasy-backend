using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.NotificationModule.Dtos.Mapper;

public class NotificationDtoMapper
{
    public static Get.NotificationDto MapNotificationToGetDto(Notification notification)
    {
        return new Get.NotificationDto
        {
            Id = notification.Id,
            Header = notification.Header,
            Content = notification.Content,
            CreatedAt = notification.CreatedAt,
            ReadAt = notification.ReadAt ?? null,
            UserId = notification.UserId
        };
    }
    
    public static Notification MapCreateDtoToNotification(Create.NotificationDto createDto)
    {
        return new Notification
        {
            Header = createDto.Header,
            Content = createDto.Content,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            UserId = createDto.UserId
        };
    }
}