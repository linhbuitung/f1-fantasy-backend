namespace F1Fantasy.Modules.AuthModule.Services.Interfaces;

public interface IAuthExtensionService
{
    Task AddFantasyLineupForUserInSeasonAsync(int userId, int year);
}