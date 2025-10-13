using F1Fantasy.Core;
using Microsoft.EntityFrameworkCore;
using F1Fantasy.Infrastructure.Contexts;
using F1Fantasy.Modules.StaticDataModule.Repositories.Implementations;
using F1Fantasy.Modules.StaticDataModule.Repositories.Interfaces;
using F1Fantasy.Modules.StaticDataModule.Services.Implementations;
using F1Fantasy.Modules.StaticDataModule.Services.Interfaces;
using F1Fantasy.Core.Common;
using Microsoft.AspNetCore.Identity;
using F1Fantasy.Core.Configurations;
using F1Fantasy.Modules.AuthModule.Extensions;
using F1Fantasy.Core.Auth;
using F1Fantasy.Core.Middlewares;
using F1Fantasy.Core.Policies;
using F1Fantasy.Infrastructure.Extensions;
using F1Fantasy.Infrastructure.ExternalServices.Implementations;
using F1Fantasy.Infrastructure.Settings;
using F1Fantasy.Modules.AdminModule.Repositories.Implementations;
using F1Fantasy.Modules.AdminModule.Repositories.Interfaces;
using F1Fantasy.Modules.AdminModule.Services.Implementations;
using F1Fantasy.Modules.AdminModule.Services.Interfaces;
using F1Fantasy.Modules.AskAiModule.Repositories.Implementations;
using F1Fantasy.Modules.AskAiModule.Repositories.Interfaces;
using F1Fantasy.Modules.AskAiModule.Services.Implementations;
using F1Fantasy.Modules.AskAiModule.Services.Interfaces;
using F1Fantasy.Modules.AuthModule.ApiMapper;
using F1Fantasy.Modules.AuthModule.Repositories.Implementations;
using F1Fantasy.Modules.AuthModule.Repositories.Interfaces;
using F1Fantasy.Modules.AuthModule.Services.Implementation;
using F1Fantasy.Modules.AuthModule.Services.Interfaces;
using F1Fantasy.Modules.CoreGameplayModule.Repositories.Implementations;
using F1Fantasy.Modules.CoreGameplayModule.Repositories.Interfaces;
using F1Fantasy.Modules.CoreGameplayModule.Services.Implementations;
using F1Fantasy.Modules.CoreGameplayModule.Services.Interfaces;
using F1Fantasy.Modules.LeagueModule.Repositories.Implementations;
using F1Fantasy.Modules.LeagueModule.Repositories.Interfaces;
using F1Fantasy.Modules.LeagueModule.Services.Implementations;
using F1Fantasy.Modules.LeagueModule.Services.Interfaces;
using F1Fantasy.Modules.UserModule.Repositories.Implementations;
using F1Fantasy.Modules.UserModule.Repositories.Interfaces;
using F1Fantasy.Modules.UserModule.Services.Implementations;
using F1Fantasy.Modules.UserModule.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Migrations;
using F1Fantasy.Infrastructure.Extensions;
using F1Fantasy.Modules.AdminModule.Extensions.Implementations;
using F1Fantasy.Modules.AdminModule.Extensions.Interfaces;
using F1Fantasy.Modules.AskAiModule.Extensions;
using F1Fantasy.Modules.NotificationModule;
using F1Fantasy.Modules.NotificationModule.Repositories.Implementations;
using F1Fantasy.Modules.NotificationModule.Repositories.Interfaces;
using F1Fantasy.Modules.NotificationModule.Services.Implementations;
using F1Fantasy.Modules.NotificationModule.Services.Interfaces;
using F1Fantasy.Modules.StatisticModule.Repositories.Implementations;
using F1Fantasy.Modules.StatisticModule.Repositories.Interfaces;
using F1Fantasy.Modules.StatisticModule.Services.Implementations;
using F1Fantasy.Modules.StatisticModule.Services.Interfaces;
using Newtonsoft.Json.Converters;

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
EnvVariableService.Load(dotenv);

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
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*?.";
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".F1Fantasy.Identity";
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
});

builder.Services.AddDbContext<WooF1Context>(options =>
    options.UseNpgsql(WooF1ContextFactory.GetConnectionString(),
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
        .ReplaceService<IHistoryRepository, WooF1HistoryRepository>()
        .UseSnakeCaseNamingConvention());

#region Auth

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddRoles<ApplicationRole>().AddEntityFrameworkStores<WooF1Context>().AddDefaultTokenProviders();
builder.Services.AddAuthorization(AuthPolicies.AddCustomPolicies);

builder.Services.AddAuthentication().AddCookie(options =>
{
    options.LoginPath = "/login"; 
    options.AccessDeniedPath = "/access-denied";
});

#endregion Auth
    
builder.Services.AddHttpClient();

builder.Services.Configure<HostOptions>(options =>
{
    options.ServicesStartConcurrently = true; // Start services concurrently
    options.ServicesStopConcurrently = false; // Stop services sequentially
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplicationScoped(); // Custom extension method to register application services

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Logging.AddConsole();
builder.Logging.AddDebug();

var corsName = "AllowAngularApp";
if (builder.Environment.IsProduction())
{
    var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<List<string>>() ?? [];
    var originsArray = allowedOrigins?.ToArray();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(corsName, policy =>
        {
            policy.WithOrigins(originsArray)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });
} 
else if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.CustomSchemaIds(type => type.FullName);
    });
    
    var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<List<string>>() ?? [];
    // Convert List<string> to string[]
    var originsArray = allowedOrigins?.ToArray();
    // Allow all cors for development
    // Get allowed origins from appsettings.Development.json
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(corsName, policy =>
        {
            policy.WithOrigins(originsArray)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });
}

var app = builder.Build();
Console.WriteLine($"Current environment: {app.Environment.EnvironmentName}");

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapIdentityApi();


// Migrate any database changes on startup 
app.MigrateDatabase<WooF1Context>();

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

app.UseCors(corsName);

app.AddMiddlewares();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/hub/notification");

app.Run();

public static class ServiceExtensions
{
    public static void AddApplicationScoped(this IServiceCollection services)
    {
        // Register application-specific services
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<IConstructorService, ConstructorService>();
        services.AddScoped<ICircuitService, CircuitService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IRaceService, RaceService>();
        services.AddScoped<IPowerupService, PowerupService> ();
        services.AddScoped<ISeasonService, SeasonService>();
        services.AddScoped<IStaticDataRepository, StaticDataRepository>();
        
        services.AddScoped<IUserService, UserService> ();
        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IAuthExtensionService, AuthExtensionService>();
        services.AddScoped<IAuthExtensionRepository, AuthExtensionRepository>();

        services.AddScoped<ILeagueService, LeagueService>();
        services.AddScoped<ILeagueRepository, LeagueRepository>();

        services.AddScoped<ICoreGameplayService, CoreGameplayService>();
        services.AddScoped<IFantasyLineupRepository, FantasyLineupRepository>();
        services.AddScoped<ICoreGameplayRepository, CoreGameplayRepository>();
        
        services.AddScoped<IAskAIService, AskAiService>();
        services.AddScoped<IAskAiRepository, AskAiRepository>();
        
        services.AddScoped<IStatisticService, StatisticService>();
        services.AddScoped<IStatisticRepository, StatisticRepository>();
        
        services.AddTransient<IEmailSender<ApplicationUser>, EmailService>();
        services.AddSingleton<ICloudStorage, GoogleCloudStorage>();
        
        services.AddSignalR();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        
        services.AddSingleton<AskAiClient>();
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
    
    public static void AddMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<UserInteractionTrackMiddleware>();
    }
}