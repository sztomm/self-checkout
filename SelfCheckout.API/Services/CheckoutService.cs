using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SelfCheckout.API.Exceptions;
using SelfCheckout.API.Interfaces;
using SelfCheckout.DAL;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SelfCheckout.API.Services
{
    public class CheckoutService : ICheckoutService
    {
        public ILogger<CheckoutService> Logger { get; }
        public SelfCheckoutDbContext DbContext { get; }

        public CheckoutService(ILogger<CheckoutService> logger, SelfCheckoutDbContext dbContext)
        {
            Logger = logger;
            DbContext = dbContext;
        }

        public async Task<int> CheckSufficiency(IDictionary<string, int> insertedMoney, int price)
        {
            Logger.LogDebug($"Checking if the inserted amount of money is sufficient");

            var count = 0;
            foreach(var inserted in insertedMoney)
                count += int.Parse(inserted.Key) * inserted.Value;

            Logger.LogDebug($"The inserted amount of money is {(count >= price ? "sufficient" : "not enough")}");
            return count - price;
        }

        public async Task<IDictionary<string, int>> CalculateMoneyBack(int moneyBack)
        {
            Logger.LogDebug($"Calculating money back");

            var stockedMoneys = await DbContext.Moneys.Include(m => m.Stock)
                                                      .Where(m => m.Value <= moneyBack && m.Stock.Count > 0)
                                                      .OrderByDescending(m => m.Value)
                                                      .ToListAsync();
            
            var splittedMoneyBack = new Dictionary<string, int>();
            foreach(var money in stockedMoneys)
            {
                while(moneyBack - money.Value >= 0 && money.Stock.Count > 0)
                {
                    moneyBack -= money.Value;
                    money.Stock.Count--;

                    if (splittedMoneyBack.ContainsKey(money.Value.ToString()))
                        splittedMoneyBack[money.Value.ToString()] += 1;
                    else
                        splittedMoneyBack.Add(money.Value.ToString(), 1);

                }

                DbContext.Update(money);

                if (moneyBack == 0)
                    break;
            }

            if (moneyBack > 0)
                throw new ImpossibleMoneyBackException($"Cannot give money back, dont have enough bills or coins");

            await DbContext.SaveChangesAsync();

            Logger.LogDebug($"Successfully calculated money back");
            return splittedMoneyBack;
        }
    }
}
