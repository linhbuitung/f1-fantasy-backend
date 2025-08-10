using F1FantasyWorker;
using F1FantasyWorker.Infrastructure.Contexts;

//using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Implementations;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Implementations;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddDbContext<WooF1Context>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddApplicationBackground();
builder.Services.AddApplicationScoped();

var host = builder.Build();
host.Run();

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationScoped(this IServiceCollection services)
    {
        // Register application-specific services
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<IConstructorService, ConstructorService>();
        services.AddScoped<ICircuitService, CircuitService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IStaticDataRepository, StaticDataRepository>();

        services.AddScoped<IF1DataSyncService, F1DataSyncService>();
        return services;
    }

    public static IServiceCollection AddApplicationBackground(this IServiceCollection services)
    {
        // Register the background service
        services.AddHostedService<GeneralSyncWorker>();

        return services;
    }
}