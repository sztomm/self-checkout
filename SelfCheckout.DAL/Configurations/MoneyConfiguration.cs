using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfCheckout.DAL.Models;

namespace SelfCheckout.DAL.Configurations
{
    public class MoneyConfiguration : BaseModelConfiguration<Money>
    {
        public override void Configure(EntityTypeBuilder<Money> builder)
        {
            base.Configure(builder);
            builder.ToTable("Moneys");

            builder.HasIndex(m => m.Value).IsUnique();

            builder.Property(m => m.Type).IsRequired();
            builder.Property(m => m.Value).IsRequired();

            builder.HasOne(m => m.Stock).WithOne(st => st.Money).HasForeignKey<Stock>(st => st.MoneyId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
