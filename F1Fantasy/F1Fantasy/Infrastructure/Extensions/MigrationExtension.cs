using F1Fantasy.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace F1Fantasy.Infrastructure.Extensions
{
    public static class MigrationExtension
    {
        public static IHost MigrateDatabase<T>(this IHost host) where T : DbContext
        {
            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var db = services.GetRequiredService<T>();
                    db.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }
            return host;
        }
    }
}