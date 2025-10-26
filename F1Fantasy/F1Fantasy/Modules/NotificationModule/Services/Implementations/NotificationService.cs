using System.ComponentModel.DataAnnotations;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.NotificationModule.Dtos.Mapper;
using F1Fantasy.Modules.NotificationModule.Repositories.Interfaces;
using F1Fantasy.Modules.NotificationModule.Services.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.UserModule.Repositories.Interfaces;
using F1Fantasy.Modules.UserModule.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace F1Fantasy.Modules.NotificationModule.Services.Implementations;

public class NotificationService(
    INotificationRepository notificationRepository, 
    IUserRepository userRepository, 
    IStaticDataRepository staticDataRepository, 
    WooF1Context context, 
    IHubContext<NotificationHub> hubContext) : INotificationService
{
    public async Task AddAndSendNotificationAsync(Dtos.Create.NotificationDto notificationDto)
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
            var user = await userRepository.GetUserByIdAsync(notificationDto.UserId);
            if(user == null)
            {
                throw new NotFoundException($"User with ID {notificationDto.UserId} not found.");
            }
            if(!user.AcceptNotification)
            {
                return;
            }
            
            var notification = NotificationDtoMapper.MapCreateDtoToNotification(notificationDto);
            var newNotification = await notificationRepository.AddNotificationAsync(notification);
            
            await context.SaveChangesAsync();
            await hubContext.Clients.Client(notificationDto.UserId.ToString()).SendAsync("ReceiveNotification",
               NotificationDtoMapper.MapNotificationToGetDto(newNotification));

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating notification: {ex.Message}");

            throw;
        }
    }

    public async Task AddAndSendNotificationForEachUserInForARaceAsync(int raceId)
    {
        var race = await staticDataRepository.GetRaceByIdAsync(raceId);
        if (race == null)
        {
            throw new NotFoundException("Race not found");
        }
        var users = await userRepository.GetAllUsersAsync();
        foreach (var user in users)
        {
            if (!user.AcceptNotification)
            {
                continue;
            }
            var notificationDto = new Dtos.Create.NotificationDto
            {
                UserId = user.Id,
                Header = "Race Results Available",
                Content =
                    $"The results for race {race.RaceName} in {race.Season.Year} have been calculated. Check out your fantasy lineup to see how you did!",
            };
            
            await AddAndSendNotificationAsync(notificationDto);

            // If user has a favorite driver or constructor in the race, customize the notification
            if (user.DriverId.HasValue)
            {
                var raceEntryDriverResult = race.RaceEntries.FirstOrDefault(re => re.DriverId == user.DriverId);
                if (raceEntryDriverResult is not null)
                {
                    var driverNotificationDto = new Dtos.Create.NotificationDto
                    {
                        UserId = user.Id,
                        Header = "Your Favorite Driver Points Update",
                        Content =
                            $" Your favorite driver {raceEntryDriverResult.Driver.GivenName} {raceEntryDriverResult.Driver.FamilyName} got {raceEntryDriverResult.PointsGained} points in latest race!",
                    };
                    await AddAndSendNotificationAsync(driverNotificationDto);

                }
            }

            
            if (user.ConstructorId.HasValue)
            {
                int constructorPoints = 0;
                var raceEntryConstructorResult = race.RaceEntries.Where(re => re.ConstructorId == user.ConstructorId).ToList();

                if (raceEntryConstructorResult.Count == 2)
                {
                    bool bothTop3 = raceEntryConstructorResult.All(p => p.Position> 0 && p.Position <= 3);
                    bool oneTop3 = raceEntryConstructorResult.Count(p => p.Position > 0 && p.Position <= 3) == 1;
                    bool bothTop10 = raceEntryConstructorResult.All(p => p.Position > 0 && p.Position <= 10);
                    bool oneTop10 = raceEntryConstructorResult.Count(p => p.Position > 0 && p.Position <= 10) == 1;

                    if (bothTop3)
                        constructorPoints += 30;
                    else if (oneTop3)
                        constructorPoints += 20;
                    else if (bothTop10)
                        constructorPoints += 15;
                    else if (oneTop10)
                        constructorPoints += 10;
                    else
                        constructorPoints += -10;
                    
                    var constructorNotificationDto = new Dtos.Create.NotificationDto
                    {
                        UserId = user.Id,
                        Header = "Your Favorite Constructor Points Update",
                        Content =
                            $" Your favorite constructor {raceEntryConstructorResult.First().Constructor.Name} got {constructorPoints} points in latest race!",
                    };
                    await AddAndSendNotificationAsync(constructorNotificationDto);
                }
            }
        }
    }

    public async Task<Dtos.Get.NotificationDto> GetNotificationAsync(int notificationId)
    {
        var notification = await notificationRepository.GetNotificationAsTrackingAsync(notificationId);
        if (notification == null)
        {
            throw new NotFoundException("Notification not found");
        }
        return NotificationDtoMapper.MapNotificationToGetDto(notification);
    }
    
    public async Task<List<Dtos.Get.NotificationDto>> GetAllUnreadNotificationsByUserIdAsync(int userId)
    {
        var notifications = await notificationRepository.GetAllUnreadNotificationsByUserIdAsTrackingAsync(userId);
        return notifications.Select(NotificationDtoMapper.MapNotificationToGetDto).ToList();    
    }

    public async Task<Dtos.Get.NotificationDto> MarkNotificationAsReadAsync(int userId, int notificationId)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // Check if user exists
            var user = await userRepository.GetUserByIdAsync(userId);
            if(user == null)
            {
                throw new NotFoundException($"User with ID {userId} not found.");
            }
            var notification = await notificationRepository.GetNotificationAsTrackingAsync(notificationId);
            if (notification == null)
            {
                throw new NotFoundException("Notification not found");
            }
            if(notification.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to mark this notification as read");
            }
            notification.ReadAt = DateTime.UtcNow;
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return NotificationDtoMapper.MapNotificationToGetDto(notification);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error updating notification: {ex.Message}");

            throw;
        }
    }

    public async Task MarkAllNotificationsAsReadAsync(int userId)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // Check if user exists
            var user = await userRepository.GetUserByIdAsync(userId);
            if(user == null)
            {
                throw new NotFoundException($"User with ID {userId} not found.");
            }
            var notification = await notificationRepository.GetAllUnreadNotificationsByUserIdAsTrackingAsync(userId);

            // Update all as read            
            foreach (var note in notification)
            {
                note.ReadAt = DateTime.UtcNow;
            }
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error updating notifications: {ex.Message}");

            throw;
        }
    }
}