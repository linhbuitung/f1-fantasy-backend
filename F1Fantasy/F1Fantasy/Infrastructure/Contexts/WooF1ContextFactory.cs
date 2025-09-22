using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using F1Fantasy.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;

namespace F1Fantasy.Infrastructure.Contexts;

public class WooF1ContextFactory : IDesignTimeDbContextFactory<WooF1Context>
{
    public WooF1Context CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WooF1Context>();
        optionsBuilder
            .UseNpgsql(GetConnectionString())
            .ReplaceService<IHistoryRepository, WooF1HistoryRepository>()
            .UseSnakeCaseNamingConvention();

        return new WooF1Context(optionsBuilder.Options);
    }

    public static string GetConnectionString()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment != null && environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            return configuration.GetConnectionString("DefaultConnection");
        }
        if (environment != null && environment.Equals("Production", StringComparison.OrdinalIgnoreCase))
        {
            var user = Environment.GetEnvironmentVariable("POSTGRES_USER");
            var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            var db = Environment.GetEnvironmentVariable("POSTGRES_DB");
            var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
            var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
            return $"Host={host};Port={port};Database={db};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        }
        throw new InvalidOperationException("ASPNETCORE_ENVIRONMENT is not set to a valid value.");
    }
}
