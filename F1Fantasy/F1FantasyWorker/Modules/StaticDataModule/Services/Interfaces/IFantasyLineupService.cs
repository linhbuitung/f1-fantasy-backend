using F1FantasyWorker.Modules.StaticDataModule.Dtos;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

public interface IFantasyLineupService
{
    Task<FantasyLineupDto> AddFantasyLineupAsyncWithNoTransaction(FantasyLineupDto fantasyLineupDto);
    
    Task AddFantasyLineupForAllUsersInASeasonAsync(int year);

    Task AddFantasyLineupForUserInSeasonAsync(int userId, int year);
}