using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.StaticDataModule.Dtos
{
    public class DriverDto
    {
        public int? Id { get; set; }

        [Required, MaxLength(300)]
        public string GivenName { get; set; }

        [Required, MaxLength(300)]
        public string FamilyName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required, MaxLength(100)]
        public string CountryId { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(300)]
        public string? ImgUrl { get; set; }

        public DriverDto(int id, string givenName, string familyName, DateTime dateOfBirth, string countryId, string code, string? imgUrl)
        {
            Id = id;
            GivenName = givenName;
            FamilyName = familyName;
            DateOfBirth = dateOfBirth;
            CountryId = countryId;
            Code = code;
            ImgUrl = imgUrl;
        }

        public DriverDto(string givenName, string familyName, DateTime dateOfBirth, string countryId, string code, string? imgUrl)
        {
            Id = null;
            GivenName = givenName;
            FamilyName = familyName;
            DateOfBirth = dateOfBirth;
            CountryId = countryId;
            Code = code;
            ImgUrl = imgUrl;
        }
    }
}