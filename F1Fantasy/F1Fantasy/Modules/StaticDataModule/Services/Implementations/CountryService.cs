using F1Fantasy.Core.Common;
using F1Fantasy.Exceptions;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Dtos;
using F1Fantasy.Modules.StaticDataModule.Dtos.Mapper;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.StaticDataModule.Services.Implementations
{
    public class CountryService(IStaticDataRepository staticDataRepository, WooF1Context context)
        : ICountryService
    {
        public async Task<CountryDto> AddCountryAsync(CountryDto countryDto)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                Country existingCountry = await staticDataRepository.GetCountryByIdAsync(countryDto.CountryId);
                if (existingCountry != null)
                {
                    return null;
                }
                // Replace special case for United States
                countryDto = ReplaceSpecialCountryCase(countryDto);

                Country country = StaticDataDtoMapper.MapDtoToCountry(countryDto);

                Country newCountry = await staticDataRepository.AddCountryAsync(country);

                // Additional operations that need atomicity (example: logging the event)
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                return StaticDataDtoMapper.MapCountryToDto(newCountry);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error creating country: {ex.Message}");

                throw;
            }
        }

        public async void AddListCountriesAsync(List<CountryDto> countryDtos)
        {
            foreach (var countryDto in countryDtos)
            {
                Console.WriteLine($"Adding country: {countryDto.CountryId}");
                await AddCountryAsync(countryDto);
            }
        }

        public async Task<CountryDto> GetCountryByIdAsync(string id)
        {
            Country? country = await staticDataRepository.GetCountryByIdAsync(id);
            if (country == null)
            {
                throw new NotFoundException($"Country with ID {id} not found");
            }
            return StaticDataDtoMapper.MapCountryToDto(country);
        }

        public async Task<CountryDto> GetCountryByNationalityAsync(string nationality)
        {
            Country? country = await staticDataRepository.GetCountryByNationalityAsync(nationality);
            if (country == null)
            {
                throw new NotFoundException($"Country with Nationality {nationality} not found");
            }
            return StaticDataDtoMapper.MapCountryToDto(country);
        }

        public async Task<CountryDto> GetCountryByShortNameAsync(string shortName)
        {
            Country? country = await staticDataRepository.GetCountryByShortNameAsync(shortName);
            if (country == null)
            {
                throw new NotFoundException($"Country with ShortName {shortName} not found");
            }
            return StaticDataDtoMapper.MapCountryToDto(country);
        }

        private CountryDto ReplaceSpecialCountryCase(CountryDto countryDto)
        {
            if (countryDto.CountryId.Equals("United States"))
            {
                countryDto.CountryId = "USA";
            }

            return countryDto;
        }
    }
}