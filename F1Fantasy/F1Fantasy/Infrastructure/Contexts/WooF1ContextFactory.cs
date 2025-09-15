using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using F1Fantasy.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;

namespace F1Fantasy.Infrastructure.Contexts
{
    public class WooF1ContextFactory : IDesignTimeDbContextFactory<WooF1Context>
    {
        public WooF1Context CreateDbContext(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Configure DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<WooF1Context>();
            optionsBuilder
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .ReplaceService<IHistoryRepository, WooF1HistoryRepository>()
                .UseSnakeCaseNamingConvention();

            return new WooF1Context(optionsBuilder.Options);
        }
    }
}