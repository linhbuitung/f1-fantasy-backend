using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;

public interface IStaticDataSearchRepository
{
    Task<(List<Driver> Drivers, int TotalCount)> SearchDriversAsync(string query, int skip, int take);

    Task<(List<Constructor> Constructors, int TotalCount)> SearchConstructorsAsync(string query, int skip, int take);

    Task<(List<Circuit> Circuits, int TotalCount)> SearchCircuitsAsync(string query, int skip, int take);

}