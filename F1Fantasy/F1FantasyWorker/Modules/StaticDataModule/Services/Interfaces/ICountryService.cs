using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces
{
    public interface ICountryService
    {
        Task<CountryDto> AddCountryAsync(CountryDto countryDto);

        void AddListCountriesAsync(List<CountryDto> countryDtos);

        Task<CountryDto> GetCountryByIdAsync(string id);

        Task<CountryDto> GetCountryByNationalityAsync(string nationality);

        Task<CountryDto> GetCountryByShortNameAsync(string shortName);
    }
}