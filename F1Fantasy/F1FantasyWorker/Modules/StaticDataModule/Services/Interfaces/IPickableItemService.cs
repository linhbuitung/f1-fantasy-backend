namespace F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

public interface IPickableItemService
{
    Task<int?> AddPickableItemAsync();
    
    Task<int?> GetPickableItemAsync();
}