namespace F1Fantasy.Modules.StaticDataModule.Dtos
{
    public class CircuitDto
    {
        public int? Id { get; set; }

        public string CircuitName { get; set; }

        public string Code { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longtitude { get; set; }

        public string Locality { get; set; }

        public string CountryId { get; set; }

        public string? ImgUrl { get; set; }
        
        public CircuitDto(int? id, string circuitName, string code, decimal latitude, decimal longtitude, string locality, string countryId, string? imgUrl)
        {
            Id = id ?? null;
            CircuitName = circuitName;
            Code = code;
            Latitude = latitude;
            Longtitude = longtitude;
            Locality = locality;
            CountryId = countryId;
            ImgUrl = imgUrl;
        }
    }
}