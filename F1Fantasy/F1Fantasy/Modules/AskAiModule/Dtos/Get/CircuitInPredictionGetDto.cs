namespace F1Fantasy.Modules.AskAiModule.Dtos.Get;

public class CircuitInPredictionGetDto
{
    public int Id { get; set; }

    public string CircuitName { get; set; }

    public string Code { get; set; }
    
    public decimal Latitude { get; set; }


    public decimal Longitude { get; set; }

    public string Locality { get; set; }

    public string CountryId { get; set; }
    
    public string? ImgUrl { get; set; }
}