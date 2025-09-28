using F1Fantasy.Modules.CoreGameplayModule.Dtos.Get;

namespace F1Fantasy.Modules.CoreGameplayModule.Services.Interfaces;

public interface ICoreGameplayService
{
    Task<Dtos.Get.FantasyLineupDto> GetFantasyLineupByIdAsync(int fantasyLineupId);
    Task<Dtos.Get.FantasyLineupDto> GetFantasyLineupByUserIdAndRaceIdAsync(int userId, int raceId);
    Task<Dtos.Get.FantasyLineupDto> GetCurrentFantasyLineupByUserIdAsync(int userId);
    Task<Dtos.Get.FantasyLineupDto> GetLatestFinishedFantasyLineupByUserIdAsync(int userId);

    Task<Dtos.Get.FantasyLineupDto> UpdateFantasyLineupAsync(Dtos.Update.FantasyLineupDto fantasyLineupDto);

    Task<Dtos.Get.FantasyLineupDto> UpdateFantasyLineupWithPowerupsAsync(Dtos.Update.FantasyLineupDto fantasyLineupDto);
    Task ResetFantasyLineupsBySeasonYearAsync(int year);
    
    Task<RaceDto> GetLatestFinishedRaceAsync();
    
    Task<RaceResultDto> GetLatestFinishedRaceResultAsync();
    
    Task<RaceDto> GetLatestRaceAsync();
    Task<RaceDto> GetCurrentRaceAsync();
    
    Task<List<PowerupDto>> GetPowerupsWithStatusBeforeCurrentRaceByUserInASeasonAsync(int userId);
    
    Task AddPowerupToCurrentLineupAsync(int userId, int powerupId);
    
    Task RemovePowerupFromCurrentLineupAsync(int userId, int powerupId);

}