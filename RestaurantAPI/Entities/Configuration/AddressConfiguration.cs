using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RestaurantAPI.Entities.Configuration
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.Property(a => a.City).HasMaxLength(50).IsRequired();
            builder.Property(a => a.Street).HasMaxLength(50).IsRequired();
        }
    }
}