using F1FantasyWorker;
using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.CoreGameplayModule.Repositories.Implementations;
using F1FantasyWorker.Modules.CoreGameplayModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.CoreGameplayModule.Services.Implementations;
using F1FantasyWorker.Modules.CoreGameplayModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Configs;

//using F1FantasyWorker.Infrastructure.Contexts;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Implementations;
using F1FantasyWorker.Modules.StaticDataModule.Repositories.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Services.Implementations;
using F1FantasyWorker.Modules.StaticDataModule.Services.Interfaces;
using F1FantasyWorker.Modules.StaticDataModule.Workers;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Implementations;
using F1FantasyWorker.Modules.StaticDataModule.Workers.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddDbContext<WooF1Context>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),  
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

builder.Services.AddApplicationBackground();
builder.Services.AddApplicationScoped();

var host = builder.Build();
host.Run();
/*
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=woof1;TrustServerCertificate=True;Username=woof1;Password=AVerySecretPassword;Include Error Detail=true" Npgsql.EntityFrameworkCore.PostgreSQL --force --context-dir "Infrastructure/Contexts" --output-dir "Core/Common" --context "WooF1Context"
 */
public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationScoped(this IServiceCollection services)
    {
        // Register application-specific services
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<IConstructorService, ConstructorService>();
        services.AddScoped<ICircuitService, CircuitService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IRaceService, RaceService>();
        services.AddScoped<IPowerupService, PowerupService> ();
        services.AddScoped<ISeasonService, SeasonService>();
        services.AddScoped<IRaceEntryService, RaceEntryService>();
        services.AddScoped<IFantasyLineupService, FantasyLineupSerivce>();
        services.AddScoped<IPickableItemService, PickableItemService>();

        services.AddScoped<IDataSyncRepository, DataSyncRepository>();

        services.AddScoped<IDriverSyncService, DriverSyncService>();
        services.AddScoped<IConstructorSyncService, ConstructorSyncService>();
        services.AddScoped<ICircuitSyncService, CircuitSyncService>();
        services.AddScoped<ICountrySyncService, CountrySyncService>();
        services.AddScoped<IRaceSyncService, RaceSyncService>();
        services.AddScoped<IPowerupSyncService, PowerupSyncService> ();
        services.AddScoped<ISeasonSyncService, SeasonSyncService>();
        services.AddScoped<IRaceEntrySyncService, RaceEntrySyncService>();
        
        services.AddScoped<ICoreGameplayRepository, CoreGameplayRepository>();
        services.AddScoped<ICoreGameplayService, CoreGameplayService>();
        
        services.AddSingleton<WorkerConfigurationService>();
        
        return services;
    }

    public static IServiceCollection AddApplicationBackground(this IServiceCollection services)
    {
        // Register the background service
        services.AddHostedService<GeneralSyncWorker>();

        return services;
    }
}