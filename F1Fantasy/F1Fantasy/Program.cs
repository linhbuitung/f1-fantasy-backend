using Microsoft.EntityFrameworkCore;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Repositories.Implementations;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Implementations;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContextPool<WooF1Context>(options =>
      options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient();

builder.Services.Configure<HostOptions>(options =>
{
    options.ServicesStartConcurrently = true; // Start services concurrently
    options.ServicesStopConcurrently = false; // Stop services sequentially
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationScoped(); // Custom extension method to register application services

builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationScoped(this IServiceCollection services)
    {
        // Register application-specific services
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<IConstructorService, ConstructorService>();
        services.AddScoped<ICircuitService, CircuitService>();
        services.AddScoped<IStaticDataRepository, StaticDataRepository>();

        return services;
    }
}