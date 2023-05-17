using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RestaurantAPI.Entities.Configuration
{
    public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
    {
        public void Configure(EntityTypeBuilder<Restaurant> builder)
        {
            builder.Property(r => r.Name).IsRequired().HasMaxLength(25);
            builder.Property(r => r.ContactEmail).IsRequired();
            builder.Property(r => r.ContactNumber).IsRequired();
        }
    }
}