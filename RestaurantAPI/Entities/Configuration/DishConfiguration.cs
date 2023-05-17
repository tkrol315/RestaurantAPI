using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RestaurantAPI.Entities.Configuration
{
    public class DishConfiguration : IEntityTypeConfiguration<Dish>
    {
        public void Configure(EntityTypeBuilder<Dish> builder)
        {
            builder.Property(d => d.Name).IsRequired();
            builder.Property(d => d.Price).IsRequired();
        }
    }
}