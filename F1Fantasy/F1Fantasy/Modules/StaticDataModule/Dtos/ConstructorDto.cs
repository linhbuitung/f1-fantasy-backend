namespace F1Fantasy.Modules.StaticDataModule.Dtos
{
    public class ConstructorDto
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string CountryId { get; set; }

        public string Code { get; set; }

        public string? ImgUrl { get; set; }
        
        public int Price { get; set; }

        public ConstructorDto(int id, string name, string countryId, string code, int price, string? imgUrl)
        {
            Id = id;
            Name = name;
            CountryId = countryId;
            Code = code;
            ImgUrl = imgUrl;
            Price = price;
        }

        public ConstructorDto(int? id, string name, string countryId, string code, int price, string? imgUrl)
        {
            Id = id ?? null;
            Name = name;
            CountryId = countryId;
            Code = code;
            ImgUrl = imgUrl;
            Price = price;
        }
    }
}