using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SelfCheckout.API.Exceptions;
using SelfCheckout.API.Interfaces;
using SelfCheckout.DAL;
using SelfCheckout.DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SelfCheckout.API.Services
{
    public class StockService : IStockService
    {
        public ILogger<StockService> Logger { get; }
        public SelfCheckoutDbContext DbContext { get; }

        public StockService(ILogger<StockService> logger, SelfCheckoutDbContext dbContext)
        {
            Logger = logger;
            DbContext = dbContext;
        }

        public async Task<IList<Money>> GetStocks()
        {
            Logger.LogDebug($"Getting moneys with stock included");

            var stocks = await DbContext.Moneys.Include(m => m.Stock).ToListAsync();

            Logger.LogDebug($"Received {stocks.Count()} different moneys from database");
            return stocks;
        }

        public async Task<(bool, string)> Validate(ICollection<string> insertedMoneyTypes)
        {
            Logger.LogDebug($"Validating inserted money types");

            var supportedMoneyTypes = await DbContext.Moneys.Select(m => m.Value.ToString()).ToListAsync();

            foreach(var insertedMoneyType in insertedMoneyTypes)
            {
                if (!supportedMoneyTypes.Contains(insertedMoneyType))
                {
                    Logger.LogWarning($"{insertedMoneyType} is not supported money type");
                    return (false, $"{insertedMoneyType} is not supported money type");
                }
            }

            Logger.LogDebug($"The inserted moneys are valid");
            return (true, "");
        }

        public async Task FillMoney(IDictionary<string, int> insertedMoneys)
        {
            Logger.LogDebug($"Filling stocked moneys with the incomming ones");

            var stockedMoneys = await DbContext.Moneys.Include(m => m.Stock).ToListAsync();
            foreach (var insertedMoney in insertedMoneys)
            {
                var stockedMoney = stockedMoneys.Single(sm => sm.Value.ToString() == insertedMoney.Key);
                if (stockedMoney.Stock.Count + insertedMoney.Value < 0)
                {
                    throw new NegativeMoneyStockException($"Cannot be taken out {insertedMoney.Value} {stockedMoney.Type}(s)");
                }

                stockedMoney.Stock.Count += insertedMoney.Value;
                DbContext.Update(stockedMoney);
            }

            await DbContext.SaveChangesAsync();
            Logger.LogDebug($"Successfully filled stocked moneys");
        }
    }
}
