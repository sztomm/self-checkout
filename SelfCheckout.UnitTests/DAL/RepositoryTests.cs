using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SelfCheckout.Abstraction.Enums;
using SelfCheckout.DAL;
using SelfCheckout.DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SelfCheckout.UnitTests.DAL
{
    [TestClass]
    public class RepositoryTests
    {
        private SelfCheckoutDbContext DbContext { get; set; }

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
        }

        [TestMethod]
        [DataRow(Type.Coin, 2)]
        [DataRow(Type.Bill, 2)]
        public async Task GetMoneys(Type type, int amount)
        {
            var storedMoneys = await DbContext.Moneys.ToListAsync();
            Assert.AreEqual(4, storedMoneys.Count(), $"Moneys were not fetched correctly");

            var moneyByType = storedMoneys.Where(m => m.Type == type).Count();
            Assert.AreEqual(moneyByType, amount, $"Expected {amount}, got {moneyByType} moneys with {nameof(type)} '{type}'");
        }

        [TestMethod]
        [DataRow(500, 5)]
        public async Task UpdateStock(int moneyValue, int amount)
        {
            var money = await DbContext.Moneys.SingleAsync(m => m.Value == moneyValue);
            money.Stock.Count += amount;

            DbContext.Update(money);
            var updatedMoney = await DbContext.Moneys.SingleAsync(m => m.Id == money.Id);
            Assert.IsTrue(updatedMoney.Stock.Count == amount, $"Money entity was not updated");
        }
    }
}
