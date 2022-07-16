using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfCheckout.DAL.Models;

namespace SelfCheckout.DAL.Configurations
{
    public class BaseModelConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseModel
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(baseModel => baseModel.Id).UseIdentityColumn(seed: 1);
        }
    }
}
