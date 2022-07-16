using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SelfCheckout.API.Configurations;
using SelfCheckout.DAL.Models;
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

        public static void SeedData(this DbContext dbContext, ILogger logger, IOptions<MoneysConfiguration> options)
        {
            var moneysConfig = options?.Value;
            if (moneysConfig == null)
            {
                logger.Error($"Cannot found any money configuration to be able to seed");
                return;
            }

            var storedMoneys = dbContext.Set<Money>().ToList();
            foreach (var money in moneysConfig.AcceptedMoneys)
            {
                if (!storedMoneys.Any(sm => sm.Type == money.Type && sm.Value == money.Value))
                {
                    dbContext.Set<Money>().Add(new() { Type = money.Type, Value = money.Value });
                }
                else {
                    logger.Verbose($"Money with {nameof(money.Type)} '{money.Type}' and {nameof(money.Value)} {money.Value} is already loaded to the database");
                }
            }

            dbContext.SaveChanges();
            logger.Information($"Successfully filled money table with the possible bills and coins");
        }
    }
}
