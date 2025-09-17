using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1FantasyWorker.Modules.StaticDataModule.Dtos
{
    public class CountryDto(string countryId, List<string> nationalities, string shortName)
    {
        [Required, MaxLength(100)]
        public string CountryId { get; set; } = countryId;

        public List<string> Nationalities { get; set; } = nationalities ?? new List<string>();

        [Required]
        public string ShortName { get; set; } = shortName;
    }
}