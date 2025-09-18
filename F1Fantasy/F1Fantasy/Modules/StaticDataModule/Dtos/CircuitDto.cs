namespace F1Fantasy.Modules.StaticDataModule.Dtos
{
    public class CircuitDto(
        int? id,
        string circuitName,
        string code,
        decimal latitude,
        decimal longitude,
        string locality,
        string countryId,
        string? imgUrl)
    {
        public int? Id { get; set; } = id ?? null;

        public string CircuitName { get; set; } = circuitName;

        public string Code { get; set; } = code;

        public decimal Latitude { get; set; } = latitude;

        public decimal Longitude { get; set; } = longitude;

        public string Locality { get; set; } = locality;

        public string CountryId { get; set; } = countryId;

        public string? ImgUrl { get; set; } = imgUrl;
    }
}