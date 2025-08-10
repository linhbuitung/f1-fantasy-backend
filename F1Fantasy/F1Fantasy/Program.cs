using Microsoft.EntityFrameworkCore;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Repositories.Implementations;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Implementations;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;
using F1Fantasy.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using F1Fantasy.Core.Common;
using Microsoft.AspNetCore.Identity;
using System;
using F1Fantasy.Core.Configurations;
using F1Fantasy.Modules.AuthModule.Extensions;
using F1Fantasy.Core.Auth;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Identity.UI.Services;
using F1Fantasy.Infrastructure.ExternalServices.Implementations;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<AuthConfiguration>(
    builder.Configuration.GetSection("AuthConfiguration"));
// Add services to the container.

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 8;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddControllers();

builder.Services.AddDbContext<WooF1Context>(options =>
      options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).UseSnakeCaseNamingConvention());

#region Auth

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<WooF1Context>().AddDefaultTokenProviders();
builder.Services.AddAuthorization();

builder.Services.AddAuthentication().AddBearerToken();

#endregion Auth

builder.Services.AddHttpClient();

builder.Services.Configure<HostOptions>(options =>
{
    options.ServicesStartConcurrently = true; // Start services concurrently
    options.ServicesStopConcurrently = false; // Stop services sequentially
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationScoped(); // Custom extension method to register application services

builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();
app.MapIdentityApi<ApplicationUser>();
//seed roles

await ServiceExtensions.SeedRoles(app.Services);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
    app.UseCors("AllowSwaggerUI");
    app.UseSwagger(); // Generates /swagger/v1/swagger.json
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
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
        services.AddScoped<INationalityService, NationalityService>();

        services.AddTransient<IEmailSender<ApplicationUser>, EmailService>();

        return services;
    }

    public static async Task SeedRoles(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var roles = new[]
        {
        new ApplicationRole { Name = AppRoles.Player, NormalizedName = AppRoles.NormalizedPlayer },
        new ApplicationRole { Name = AppRoles.Admin, NormalizedName = AppRoles.NormalizedAdmin },
        new ApplicationRole { Name = AppRoles.SuperAdmin, NormalizedName = AppRoles.NormalizedSuperAdmin },
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name))
            {
                await roleManager.CreateAsync(role);
            }
        }
    }
}