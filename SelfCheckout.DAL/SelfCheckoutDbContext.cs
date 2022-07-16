using Microsoft.EntityFrameworkCore;
using SelfCheckout.DAL.Configurations;
using SelfCheckout.DAL.Models;
using System.Reflection;

namespace SelfCheckout.DAL
{
    public class SelfCheckoutDbContext : DbContext
    {
        public DbSet<Money> Moneys { get; set; }
        public DbSet<Stock> Stocks { get; set; }

        public SelfCheckoutDbContext(DbContextOptions<SelfCheckoutDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(BaseModelConfiguration<BaseModel>)));
        }
    }
}
