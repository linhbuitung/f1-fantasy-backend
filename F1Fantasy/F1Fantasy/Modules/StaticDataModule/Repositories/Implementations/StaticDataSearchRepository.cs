using F1Fantasy.Core.Common;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Modules.StaticDataModule.Repositories.Implementations;

public class StaticDataSearchRepository(WooF1Context context) :IStaticDataSearchRepository
{
    public async Task<(List<Driver> Drivers, int TotalCount)> SearchDriversAsync(string query, int skip, int take)
    {
        var driverQuery = context.Drivers
            .Where(d =>
                EF.Functions.ILike(string.Concat(d.GivenName, " ", d.FamilyName), $"%{query}%") ||
                EF.Functions.ILike(d.Code, $"%{query}%"));

        var totalCount = await driverQuery.CountAsync();
        var drivers = await driverQuery
            .OrderBy(d => d.GivenName)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return (drivers, totalCount);
    }

    public async Task<(List<Constructor> Constructors, int TotalCount)> SearchConstructorsAsync(string query, int skip,
        int take)
    {
        var constructorsQuery = context.Constructors
            .Where(c =>
                EF.Functions.ILike(c.Name, $"%{query}%") ||
                EF.Functions.ILike(c.Code, $"%{query}%"));

        var totalCount = await constructorsQuery.CountAsync();
        var constructors = await constructorsQuery
            .OrderBy(c => c.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return (constructors, totalCount);
    }

    public async Task<(List<Circuit> Circuits, int TotalCount)> SearchCircuitsAsync(string query, int skip, int take)
    {
        var circuitsQuery = context.Circuits
            .Where(c =>
                EF.Functions.ILike(c.CircuitName, $"%{query}%") ||
                EF.Functions.ILike(c.Code, $"%{query}%"));

        var totalCount = await circuitsQuery.CountAsync();
        var circuits = await circuitsQuery
            .OrderBy(c => c.CircuitName)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return (circuits, totalCount);
    }
}