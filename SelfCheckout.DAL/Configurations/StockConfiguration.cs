using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfCheckout.DAL.Models;

namespace SelfCheckout.DAL.Configurations
{
    public class StockConfiguration : BaseModelConfiguration<Stock>
    {
        public override void Configure(EntityTypeBuilder<Stock> builder)
        {
            base.Configure(builder);
            builder.ToTable("Stocks");

            builder.Property(st => st.Count).IsRequired().HasDefaultValue(0);
        }
    }
}
