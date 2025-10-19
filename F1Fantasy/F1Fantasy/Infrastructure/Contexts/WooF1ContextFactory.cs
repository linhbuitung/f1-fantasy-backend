using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using F1Fantasy.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;

namespace F1Fantasy.Infrastructure.Contexts;

public class WooF1ContextFactory : IDesignTimeDbContextFactory<WooF1Context>
{
    public WooF1Context CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WooF1Context>();
        optionsBuilder
            .UseNpgsql(GetConnectionString(),  
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
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
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            if(databaseUrl == null)
            {
                throw new InvalidOperationException("DATABASE_URL is not set.");
            }
            var databaseUri = new Uri(databaseUrl);

            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
            };

            return builder.ToString();
        }
        throw new InvalidOperationException("ASPNETCORE_ENVIRONMENT is not set to a valid value.");
    }
}
