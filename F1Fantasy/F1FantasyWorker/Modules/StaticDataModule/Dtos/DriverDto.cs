using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1FantasyWorker.Modules.StaticDataModule.Dtos
{
    public class DriverDto
    {
        public Guid? Id { get; set; }

        [Required, MaxLength(300)]
        public string GivenName { get; set; }

        [Required, MaxLength(300)]
        public string FamilyName { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required, MaxLength(200)]
        public string Nationality { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(300)]
        public string? ImgUrl { get; set; }

        public DriverDto(Guid id, string givenName, string familyName, DateOnly dateOfBirth, string nationality, string code, string? imgUrl)
        {
            Id = id;
            GivenName = givenName;
            FamilyName = familyName;
            DateOfBirth = dateOfBirth;
            Nationality = nationality;
            Code = code;
            ImgUrl = imgUrl;
        }

        public DriverDto(string givenName, string familyName, DateOnly dateOfBirth, string nationality, string code, string? imgUrl)
        {
            Id = null;
            GivenName = givenName;
            FamilyName = familyName;
            DateOfBirth = dateOfBirth;
            Nationality = nationality;
            Code = code;
            ImgUrl = imgUrl;
        }
    }
}