using Captive.Data;
using Microsoft.EntityFrameworkCore;

namespace Captive.Commands.Extensions
{
    public static class DatabaseMigration
    {
        public async static Task RunDatabaseMigration(this WebApplication app)
        {
            using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<CaptiveDataContext>();

                if (context == null)
                    throw new Exception("Database context can't be null");

                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

                if (pendingMigrations.Any())
                {
                    await context.Database.MigrateAsync();
                }
            }
        }
    }
}
