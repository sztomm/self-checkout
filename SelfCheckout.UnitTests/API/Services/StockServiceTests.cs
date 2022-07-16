using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SelfCheckout.Abstraction.Enums;
using SelfCheckout.API.Services;
using SelfCheckout.DAL;
using SelfCheckout.DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SelfCheckout.UnitTests.API.Services
{
    [TestClass]
    public class StockServiceTests
    {
        private SelfCheckoutDbContext DbContext { get; set; }
        private StockService StockService { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            var dbOptions = new DbContextOptionsBuilder<SelfCheckoutDbContext>().UseInMemoryDatabase("SelfCheckoutStore").Options;
            DbContext = new SelfCheckoutDbContext(dbOptions);

            if (DbContext.Moneys.Any())
            {
                DbContext.Moneys.RemoveRange(DbContext.Moneys.ToList());
            }

            DbContext.Moneys.Add(new Money() { Type = Type.Coin, Value = 1 });
            DbContext.Moneys.Add(new Money() { Type = Type.Coin, Value = 5 });
            DbContext.Moneys.Add(new Money() { Type = Type.Bill, Value = 500 });
            DbContext.Moneys.Add(new Money() { Type = Type.Bill, Value = 1000 });

            DbContext.SaveChanges();

            StockService = new StockService(NullLogger<StockService>.Instance, DbContext);
        }

        [TestMethod]
        public async Task GetMoneysTest()
        {
            var stockedMoneys = await StockService.GetStocks();
            var stockedMoneysCount = DbContext.Moneys.Count();
            Assert.IsTrue(stockedMoneys.Count() == stockedMoneysCount, $"Stocked moneys were not fetched correctly. Expected {stockedMoneysCount}, got {stockedMoneys.Count()}");
        }
    }
}
