using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations;

public class PickableItemService(IDataSyncRepository dataSyncRepository, WooF1Context context) : IPickableItemService
{
    public async Task<int?> AddPickableItemAsync()
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var exisitngPickableItem = await dataSyncRepository.GetPickableItemAsync();
            if (exisitngPickableItem != null)
            {
                return null;
            }

            var item = await dataSyncRepository.AddPickableItemAsync();
            
            // Additional operations that need atomicity (example: logging the event)
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            return item.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating pickable item: {ex.Message}");

            throw;
        }
    }

    public async Task<int?> GetPickableItemAsync()
    {
        var item = await dataSyncRepository.GetPickableItemAsync();
        if (item == null)
        {
            return null;
        }

        return item.Id;
    }
}