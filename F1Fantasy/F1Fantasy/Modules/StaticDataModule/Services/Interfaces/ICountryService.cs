using F1Fantasy.Modules.StaticDataModule.Dtos;

namespace F1Fantasy.Modules.StaticDataModule.Services.Interfaces
{
    public interface ICountryService
    {
        Task<CountryDto> AddCountryAsync(CountryDto countryDto);

        void AddListCountriesAsync(List<CountryDto> countryDtos);

        Task<CountryDto> GetCountryByIdAsync(string id);

        Task<CountryDto> GetCountryByNationalityAsync(string nationality);

        Task<CountryDto> GetCountryByShortNameAsync(string shortName);
        
        Task<List<CountryDto>> GetAllCountriesAsync();
    }
}