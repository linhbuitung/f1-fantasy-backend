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
}