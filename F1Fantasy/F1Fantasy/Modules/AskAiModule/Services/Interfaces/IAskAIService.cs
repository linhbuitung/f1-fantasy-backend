namespace F1Fantasy.Modules.AskAiModule.Services.Interfaces;

public interface IAskAIService
{
    Task AddAskAiCreditAsync(int userId);
    Task AddAskAiCreditAsync(string userId);
    
    Task<List<Dtos.Get.PredictionGetDto>> GetPagedPredictionsByUserIdAsync(int userId, int pageNumber, int pageSize);
    
    Task <Dtos.Get.PredictionGetDto> GetPredictionDetailByIdAsync(int predictionId);
    
    // Returns the Id of the created prediction
    Task<int> MakeMainRacePredictionAsNewPredictionAsync(int userId,
        Dtos.Create.MainRacePredictionCreateAsNewDto mainRacePredictionCreateAsNewDto);
    
    // Returns the Id of the created prediction
    Task <int> MakeQualifyingPredictionAsync(int userId, Dtos.Create.QualifyingPredictionCreateDto qualifyingPredictionCreateDto);

    Task <int> MakeMainRacePredictionFromAlreadyMadeQualifyingPredictionAsync(int userId, int predictionId, Dtos.Create.MainRacePredictionCreateAsAdditionDto mainRacePredictionCreateAsAdditionDto);
}