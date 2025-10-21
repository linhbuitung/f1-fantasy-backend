using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.AskAiModule.Repositories.Interfaces;

public interface IAskAiRepository
{
    Task<List<Prediction>> GetAllPredictionsByUserIdAsync(int userId);
    
    Task<Prediction?> GetPredictionDetailByIdAsync(int predictionId);
    Task<Prediction?> GetPredictionDetailByIdAsTrackingAsync(int predictionId);
    
    Task<Prediction>  AddPredictionAsync(Prediction prediction);
    
    Task AddDriverPredictionsAsync(List<DriverPrediction> driverInPredictions);
}