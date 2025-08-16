using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.StaticDataModule.Dtos
{
    public class CountryDto
    {
        [Required, MaxLength(100)]
        public string CountryId { get; set; }

        public List<string> Nationalities { get; set; }

        [Required]
        public string ShortName { get; set; }

        public CountryDto(string countryId, List<string>nationalities, string shortName)
        {
            CountryId = countryId;
            Nationalities = nationalities ?? new List<string>();
            ShortName = shortName;
        }
    }
}