namespace F1Fantasy.Modules.AskAiModule.Extensions;

public class AskAiClient : HttpClient
{
    private readonly string _mlUrl;

    public AskAiClient(IConfiguration configuration)
    {
        var url = configuration.GetValue<string>("CoreGameplaySettings:AskAiSettings:AskAiUrl");
        if (string.IsNullOrEmpty(url))
        {
            throw new Exception("AI service URL is not configured");
        }

        this._mlUrl = url;
        
        var apiKey = Environment.GetEnvironmentVariable("ML_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("ML API Key is not configured in environment variables");
        }
        DefaultRequestHeaders.Add("Ml-API-Key", apiKey);
    }
    
    public async Task<Dtos.Api.MainRacePredictResponseDto?> CallMainRacePrediction( List<Dtos.Api.MainRacePredictInputDto> apiInputDto)
    {
        var response = await this.PostAsJsonAsync($"{_mlUrl}/main-race/predict/batch", apiInputDto);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to get prediction from AI service");
        }
        var predictionResults = await response.Content.ReadFromJsonAsync<Dtos.Api.MainRacePredictResponseDto>();
        return predictionResults;
    }
    
    public async Task<Dtos.Api.QualifyingPredictResponseDto?> CallQualifyingPrediction( List<Dtos.Api.QualifyingPredictInputDto> apiInputDto)
    {
        var response = await this.PostAsJsonAsync($"{_mlUrl}/qualifying/predict/batch", apiInputDto);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to get prediction from AI service");
        }
        var predictionResults = await response.Content.ReadFromJsonAsync<Dtos.Api.QualifyingPredictResponseDto>();
        return predictionResults;
    }
    
    public async Task<Dtos.Api.StatusPredictResponseDto?> CallStatusPrediction( List<Dtos.Api.StatusPredictInputDto> apiInputDto)
    {
        var response = await this.PostAsJsonAsync($"{_mlUrl}/status/predict/batch", apiInputDto);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to get prediction from AI service");
        }
        var predictionResults = await response.Content.ReadFromJsonAsync<Dtos.Api.StatusPredictResponseDto>();
        return predictionResults;
    }
    
    public enum PredictionType
    {
        MainRace,
        Qualifying,
        Status
    }
    
    private string GetPredictionTypeSuffix(PredictionType predictionType)
    {
        return predictionType switch
        {
            PredictionType.MainRace => "main-race",
            PredictionType.Qualifying => "qualifying",
            PredictionType.Status => "status",
            _ => throw new ArgumentOutOfRangeException(nameof(predictionType), predictionType, null)
        };
    }
    
    public async Task<List<Dtos.Api.PickableDriverApiDto>?> GetPickableDriversAsync(PredictionType predictionType)
    {
        var typeSuffix = GetPredictionTypeSuffix(predictionType);
        var response = await this.GetAsync($"{_mlUrl}/{typeSuffix}/options/drivers");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to get pickable drivers from AI service");
        }
        var drivers = await response.Content.ReadFromJsonAsync<List<Dtos.Api.PickableDriverApiDto>>();
        return drivers;
    }
    
    public async Task<List<Dtos.Api.PickableConstructorApiDto>?> GetPickableConstructorsAsync(PredictionType predictionType)
    {
        var typeSuffix = GetPredictionTypeSuffix(predictionType);
        var response = await this.GetAsync($"{_mlUrl}/{typeSuffix}/options/constructors");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to get pickable constructors from AI service");
        }
        var constructors = await response.Content.ReadFromJsonAsync<List<Dtos.Api.PickableConstructorApiDto>>();
        return constructors;
    }
    
    public async Task<List<Dtos.Api.PickableCircuitApiDto>?> GetPickableCircuitsAsync(PredictionType predictionType)
    {
        var typeSuffix = GetPredictionTypeSuffix(predictionType);
        var response = await this.GetAsync($"{_mlUrl}/{typeSuffix}/options/circuits");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to get pickable circuits from AI service");
        }
        var circuits = await response.Content.ReadFromJsonAsync<List<Dtos.Api.PickableCircuitApiDto>>();
        return circuits;
    }
}