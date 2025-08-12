using F1FantasyWorker.Modules.StaticDataModule.Dtos;
using F1FantasyWorker.Modules.StaticDataModule.Workers.TempModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1FantasyWorker.Modules.StaticDataModule.Workers.Services
{
    public interface IF1DataSyncService
    {
        Task<List<DriverApiDto>> GetDriversAsync();

        Task<List<ConstructorApiDto>> GetConstructorsAsync();

        Task<List<CircuitApiDto>> GetCircuitsAsync();

        Task<List<CountryDto>> GetNationalitiesAsync();

        Task<List<RaceApiDto>> GetRacesAsync();
    }
}