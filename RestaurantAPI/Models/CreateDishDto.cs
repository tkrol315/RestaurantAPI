using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class CreateDishDto
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int RestaurantId { get; set; }
    }
}