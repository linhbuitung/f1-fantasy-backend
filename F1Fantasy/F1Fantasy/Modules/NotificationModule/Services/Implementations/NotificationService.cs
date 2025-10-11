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

public class NotificationService(INotificationRepository notificationRepository, IUserRepository userRepository, IStaticDataRepository staticDataRepository, WooF1Context context, IHubContext<NotificationHub> hubContext) : INotificationService
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
            var notification = NotificationDtoMapper.MapCreateDtoToNotification(notificationDto);
            var newNotification = await notificationRepository.AddNotificationAsync(notification);
            
            await context.SaveChangesAsync();
            await hubContext.Clients.Client(notificationDto.UserId.ToString()).SendAsync("ReceiveNotification",
                new { Title = notificationDto.Header, Content = notificationDto.Content });

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating notification: {ex.Message}");

            throw;
        }
    }

    public async Task AddNotificationForEachUserInForARaceAsync(int raceId)
    {
        var race = await staticDataRepository.GetRaceByIdAsync(raceId);
        if (race == null)
        {
            throw new NotFoundException("Race not found");
        }
        var users = await userRepository.GetAllUsersAsync();
        foreach (var user in users)
        {
            var notificationDto = new Dtos.Create.NotificationDto
            {
                UserId = user.Id,
                Header = "Race Results Available",
                Content =
                    $"The results for race {race.RaceName} in {race.Season.Year} have been calculated. Check out your fantasy lineup to see how you did!",
            };
            
            // If user has a favorite driver or constructor in the race, customize the notification
            if (user.DriverId.HasValue)
            {
                var raceEntryDriverResult = race.RaceEntries.FirstOrDefault(re => re.DriverId == user.DriverId);
                if (raceEntryDriverResult is not null)
                {
                    notificationDto.Content += $" Your favorite driver {raceEntryDriverResult.Driver.GivenName} {raceEntryDriverResult.Driver.FamilyName} got {raceEntryDriverResult.PointsGained} not accounting for any powerup!";
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
                    
                    notificationDto.Content += $" Your favorite constructor {raceEntryConstructorResult.First().Constructor.Name} got {constructorPoints} not accounting for any powerup!";
                }
            }
            
            await AddAndSendNotificationAsync(notificationDto);
        }
    }
}