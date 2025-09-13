using F1FantasyWorker.Modules.StaticDataModule.Dtos;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Interfaces;

public interface ICountrySyncService
{
    Task<List<CountryDto>> GetCountriesFromStaticResourcesAsync();
}