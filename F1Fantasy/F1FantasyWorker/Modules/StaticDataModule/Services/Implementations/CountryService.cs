using F1FantasyWorker.Core.Common;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Dtos.Mapper;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace F1FantasyWorker.Modules.StaticDataModule.Services.Implementations
{
    public class CountryService : ICountryService
    {
        private readonly IDataSyncRepository _dataSyncRepository;
        private readonly WooF1Context _context;

        public CountryService(IDataSyncRepository dataSyncRepository, WooF1Context context)
        {
            _dataSyncRepository = dataSyncRepository;
            _context = context;
        }

        public async Task<CountryDto> AddCountryAsync(CountryDto countryDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Country existingCountry = await _dataSyncRepository.GetCountryByIdAsync(countryDto.CountryId);
                if (existingCountry != null)
                {
                    return null;
                }
                // Replace special case for United States
                countryDto = ReplaceSpecialCountryCase(countryDto);

                Country country = StaticDataDtoMapper.MapDtoToCountry(countryDto);

                Country newCountry = await _dataSyncRepository.AddCountryAsync(country);

                // Additional operations that need atomicity (example: logging the event)
                await _context.SaveChangesAsync();

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
            Country country = await _dataSyncRepository.GetCountryByIdAsync(id);
            if (country == null)
            {
                return null;
            }
            return StaticDataDtoMapper.MapCountryToDto(country);
        }

        public async Task<CountryDto> GetCountryByNationalityAsync(string nationality)
        {
            Country country = await _dataSyncRepository.GetCountryByNationalitityAsync(nationality);
            if (country == null)
            {
                return null;
            }
            return StaticDataDtoMapper.MapCountryToDto(country);
        }

        public async Task<CountryDto> GetCountryByShortNameAsync(string shortName)
        {
            Country country = await _dataSyncRepository.GetCountryByShortNameAsync(shortName);
            if (country == null)
            {
                return null;
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