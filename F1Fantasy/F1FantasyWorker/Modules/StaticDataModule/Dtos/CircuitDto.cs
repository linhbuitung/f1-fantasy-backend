using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1FantasyWorker.Modules.StaticDataModule.Dtos
{
    public class CircuitDto
    {
        public Guid? Id { get; set; }

        public string CircuitName { get; set; }

        public string Code { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longtitude { get; set; }

        public string Locality { get; set; }

        public string Country { get; set; }

        public string? ImgUrl { get; set; }

        public CircuitDto(string circuitName, string code, decimal latitude, decimal longtitude, string locality, string country, string? imgUrl)
        {
            Id = null;
            CircuitName = circuitName;
            Code = code;
            Latitude = latitude;
            Longtitude = longtitude;
            Locality = locality;
            Country = country;
            ImgUrl = imgUrl;
        }

        public CircuitDto(Guid id, string circuitName, string code, decimal latitude, decimal longtitude, string locality, string country, string? imgUrl)
        {
            Id = id;
            CircuitName = circuitName;
            Code = code;
            Latitude = latitude;
            Longtitude = longtitude;
            Locality = locality;
            Country = country;
            ImgUrl = imgUrl;
        }
    }
}