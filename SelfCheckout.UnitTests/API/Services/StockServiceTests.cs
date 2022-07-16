using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SelfCheckout.Abstraction.Enums;
using SelfCheckout.API.Exceptions;
using SelfCheckout.API.Services;
using SelfCheckout.DAL;
using SelfCheckout.DAL.Models;
using System.Collections.Generic;
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

        [TestMethod]
        [DataRow(new string[] { "5", "1000" }, true)]
        [DataRow(new string[] { "5", "1000", "100000" }, false)]
        public async Task TestValidation(string[] moneyValues, bool isValidExpected)
        {
            (bool isValid, _) = await StockService.Validate(moneyValues);
            Assert.AreEqual(isValidExpected, isValid, $"Expected {isValidExpected}, got {isValid}");
        }

        [TestMethod]
        [DataRow("5", 5, false)]
        [DataRow("500", 1, false)]
        [DataRow("1000", 10, false)]
        [DataRow("1000", -10, true)]
        public async Task FillMoneyTest(string moneyValue, int amount, bool throwsException)
        {
            var insetMoneys = new Dictionary<string, int>()
            {
                { moneyValue, amount }
            };

            if (throwsException)
            {
                await Assert.ThrowsExceptionAsync<NegativeMoneyStockException>(async () => await StockService.FillMoney(insetMoneys));
            }
            else
            {
                await StockService.FillMoney(insetMoneys);
                var moneyType = DbContext.Moneys.Include(m => m.Stock).Single(m => m.Value.ToString() == moneyValue);
                Assert.AreEqual(amount, moneyType.Stock.Count, $"Expected {amount}, got {moneyType.Stock.Count}");
            }
        }
    }
}
