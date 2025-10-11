using System.ComponentModel.DataAnnotations;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.NotificationModule.Dtos.Mapper;
using F1Fantasy.Modules.NotificationModule.Repositories.Interfaces;
using F1Fantasy.Modules.NotificationModule.Services.Interfaces;
using F1Fantasy.Modules.UserModule.Services.Interfaces;

namespace F1Fantasy.Modules.NotificationModule.Services.Implementations;

public class NotificationService(INotificationRepository notificationRepository, IUserService userService, WooF1Context context) : INotificationService
{
    public async Task<Dtos.Get.NotificationDto> AddNotificationAsync(Dtos.Create.NotificationDto notificationDto)
    {
        var validationContext = new ValidationContext(notificationDto);
        var results = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(notificationDto, validationContext, results, true);

        if (!isValid)
        {
            var errors = string.Join(", ", results.Select(r => r.ErrorMessage));
            throw new ValidationException($"NotificationDto validation failed: {errors}");
        }
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // Check if user exists
            _ = await userService.GetUserByIdAsync(notificationDto.UserId);
            var notification = NotificationDtoMapper.MapCreateDtoToNotification(notificationDto);
            var newNotification = await notificationRepository.AddNotificationAsync(notification);
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return NotificationDtoMapper.MapNotificationToGetDto(newNotification);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating notification: {ex.Message}");

            throw;
        }
    }
}