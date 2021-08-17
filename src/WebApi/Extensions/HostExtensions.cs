using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using WebApi.Infrastructure.Data;

namespace WebApi.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            try
            {
                using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var applied = dbContext.GetService<IHistoryRepository>()
                    .GetAppliedMigrations()
                    .Select(m => m.MigrationId);

                var total = dbContext.GetService<IMigrationsAssembly>()
                    .Migrations.Select(m => m.Key);

                var hasPendingMigration = total.Except(applied).Any();
                if (hasPendingMigration)
                    dbContext.Database.Migrate();

                var dbSeeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
                dbSeeder.Seed();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                Console.WriteLine(ex);
                throw;
            }

            return host;
        }
    }
}