using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.StaticDataModule.Dtos
{
    public class CountryDto(string countryId, List<string> nationalities, string shortName)
    {
        [Required, MaxLength(100)]
        public string Id { get; set; } = countryId;

        public List<string> Nationalities { get; set; } = nationalities ?? new List<string>();

        [Required]
        public string ShortName { get; set; } = shortName;
    }
}