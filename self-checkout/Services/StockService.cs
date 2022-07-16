using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

            var stocks = await DbContext.Moneys.ToListAsync();

            Logger.LogDebug($"Received {stocks.Count()} different moneys from database");
            return stocks;
        }
    }
}
