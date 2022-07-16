using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Linq;

namespace SelfCheckout.API.Extensions
{
    public static class DbContextExtensions
    {
        public static void MigrateDb(this DbContext dbContext, ILogger logger)
        {
            try
            {
                logger.Information($"Checking database for applied migrations");
                var appliedMigrations = dbContext.Database.GetAppliedMigrations().OrderBy(x => x).ToArray();
                var existingMigrations = dbContext.Database.GetMigrations().OrderBy(x => x).ToArray();
                if (!appliedMigrations.SequenceEqual(existingMigrations))
                {
                    logger.Information($"Applying new migrations");
                    dbContext.Database.Migrate();
                    logger.Information($"Successfully migrated database to the latest state");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Automatic migration failed on database");
            }
        }
    }
}
