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
    public class CheckoutServiceTests
    {
        private SelfCheckoutDbContext DbContext { get; set; }
        private CheckoutService CheckoutService { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            var dbOptions = new DbContextOptionsBuilder<SelfCheckoutDbContext>().UseInMemoryDatabase("SelfCheckoutStore").Options;
            DbContext = new SelfCheckoutDbContext(dbOptions);

            if (DbContext.Moneys.Any())
            {
                DbContext.Moneys.RemoveRange(DbContext.Moneys.ToList());
            }

            DbContext.Moneys.Add(new Money() { Type = Type.Coin, Value = 5, Stock = new() { Count = 10 } });
            DbContext.Moneys.Add(new Money() { Type = Type.Coin, Value = 200, Stock = new() { Count = 1 } });
            DbContext.Moneys.Add(new Money() { Type = Type.Bill, Value = 500, Stock = new() { Count = 2 } });
            DbContext.Moneys.Add(new Money() { Type = Type.Bill, Value = 1000, Stock = new() { Count = 10 } });

            DbContext.SaveChanges();

            CheckoutService = new CheckoutService(NullLogger<CheckoutService>.Instance, DbContext);
        }

        [TestMethod]
        [DataRow("1000", 5, 3200, true)]
        [DataRow("10000", 1, 11000, false)]
        public async Task CheckSufficiencyTest(string money, int amount, int price, bool expectedSufficiency)
        {
            var insertedMoney = new Dictionary<string, int>()
            {
                { money, amount }
            };

            var isSufficient = await CheckoutService.CheckSufficiency(insertedMoney, price);
            Assert.IsTrue(expectedSufficiency == isSufficient >= 0, $"Expected {expectedSufficiency}, got {isSufficient >= 0}");
        }

        [TestMethod]
        [DataRow(1250, false)]
        [DataRow(12500, true)]
        public async Task CalculateMoneyBackTest(int moneyBack, bool throwsException)
        {
            if (throwsException)
            {
                await Assert.ThrowsExceptionAsync<ImpossibleMoneyBackException>(async () => await CheckoutService.CalculateMoneyBack(moneyBack));
            }
            else
            {
                var splittedMoneyBack = await CheckoutService.CalculateMoneyBack(moneyBack);
                Assert.IsTrue(true);
            }
        }
    }
}
