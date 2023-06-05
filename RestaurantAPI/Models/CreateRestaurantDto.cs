﻿using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class CreateRestaurantDto
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public string Category { get; set; }
        public bool HasDelivery { get; set; }

        public string ContactEmail { get; set; }

        [Phone]
        public string ContactNumber { get; set; }

        public string City { get; set; }
        public string Street { get; set; }

        public string PostalCode { get; set; }
    }
}