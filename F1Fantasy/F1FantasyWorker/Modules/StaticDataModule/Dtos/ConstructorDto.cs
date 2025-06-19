//using F1FantasyWorker.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1FantasyWorker.Modules.StaticDataModule.Dtos
{
    public class ConstructorDto
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string Nationality { get; set; }

        public string Code { get; set; }

        public string? ImgUrl { get; set; }

        public ConstructorDto(Guid id, string name, string nationality, string code, string? imgUrl)
        {
            Id = id;
            Name = name;
            Nationality = nationality;
            Code = code;
            ImgUrl = imgUrl;
        }

        public ConstructorDto(string name, string nationality, string code, string? imgUrl)
        {
            Id = null;
            Name = name;
            Nationality = nationality;
            Code = code;
            ImgUrl = imgUrl;
        }
    }
}