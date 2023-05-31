﻿using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Entities
{
    public class Dish
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }
    }
}