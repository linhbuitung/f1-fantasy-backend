using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.AskAiModule.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.AskAiModule.Repositories.Implementations;

public class AskAiRepository( WooF1Context context) : IAskAiRepository
{
    public async Task<List<Prediction>> GetAllPredictionsByUserIdAsync(int userId)
    {
         return await context.Predictions
            .Include(p => p.DriverPredictions)
            .Include(p => p.Circuit)
            .Where(p => p.UserId == userId).ToListAsync();
    }

    public async Task<Prediction?> GetPredictionDetailByIdAsync(int predictionId)
    {
        return await context.Predictions
            .Include(p => p.DriverPredictions)
            .ThenInclude(dp => dp.Driver)
            .Include(p => p.DriverPredictions)
            .ThenInclude(dp => dp.Constructor)
            .Include(p => p.Circuit)
            .FirstOrDefaultAsync(p => p.Id == predictionId);
    }

    public async Task<Prediction> AddPredictionAsync(Prediction prediction)
    {
        context.Predictions.Add(prediction);
        await context.SaveChangesAsync();
        return prediction;
    }

    public async Task AddDriverPredictionsAsync(List<DriverPrediction> driverInPredictions)
    {
        context.DriverPredictions.AddRange(driverInPredictions);
        await context.SaveChangesAsync();
    }
}