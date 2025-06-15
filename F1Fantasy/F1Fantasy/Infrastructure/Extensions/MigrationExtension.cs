using F1Fantasy.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace F1Fantasy.Infrastructure.Extensions
{
    public class MigrationExtension
    {
        public static void ApplyMigration(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<WooF1Context>();
                if (context.Database.IsRelational())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}