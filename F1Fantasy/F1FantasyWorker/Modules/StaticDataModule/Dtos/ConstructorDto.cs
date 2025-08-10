//using F1FantasyWorker.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1FantasyWorker.Modules.StaticDataModule.Dtos
{
    public class ConstructorDto
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string CountryId { get; set; }

        public string Code { get; set; }

        public string? ImgUrl { get; set; }

        public ConstructorDto(int id, string name, string countryId, string code, string? imgUrl)
        {
            Id = id;
            Name = name;
            CountryId = countryId;
            Code = code;
            ImgUrl = imgUrl;
        }

        public ConstructorDto(string name, string countryId, string code, string? imgUrl)
        {
            Id = null;
            Name = name;
            CountryId = countryId;
            Code = code;
            ImgUrl = imgUrl;
        }
    }
}