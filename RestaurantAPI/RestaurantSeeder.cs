using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI
{
    public class RestaurantSeeder
    {
        private readonly RestaurantDbContext _dbContext;

        public RestaurantSeeder(RestaurantDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                var pendingMigrations = _dbContext.Database.GetPendingMigrations();
                if (pendingMigrations != null && pendingMigrations.Any())
                {
                    _dbContext.Database.Migrate();
                }
                if (!_dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    _dbContext.AddRange(roles);
                    _dbContext.SaveChanges();
                }

                if (!_dbContext.Restaurants.Any())
                {
                    var restaurants = GetRestaurants();
                    _dbContext.Restaurants.AddRange(restaurants);
                    _dbContext.SaveChanges();
                }
            }
        }

        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new Role(){Name="User"},
                new Role(){Name = "Manager"},
                new Role(){Name = "Admin"}
            };
            return roles;
        }

        private IEnumerable<Restaurant> GetRestaurants()
        {
            var restaurants = new List<Restaurant>()
                {
                    new Restaurant()
                    {
                        Name = "KFC",
                        Category = "Fast Food",
                        Description = "KFC - Kentucky Fried Chicken is an American fast food restaurant",
                        ContactEmail = "contact@kfc.com",
                        ContactNumber = "111111111",
                        HasDelivery = true,
                        Dishes = new List<Dish>()
                        {
                            new Dish()
                            {
                                Name = "15 Strips",
                                Price  = 30.99M,
                            },
                            new Dish()
                            {
                                Name = "B-Smart with longer",
                                Price = 7.50M,
                            }
                        },
                        Address = new Address()
                        {
                            City = "Warsaw",
                            Street = "Złota 15",
                            PostalCode = "00-002"
                        }
                    },
                    new Restaurant()
                    {
                        Name = "McDonalds",
                        Category = "Fast Food",
                        Description = "McDonalds is an American fast food restaurant",
                        ContactEmail = "contact@mcdonalds.com",
                        ContactNumber = "222222222",
                        HasDelivery = true,
                        Dishes = new List<Dish>()
                        {
                            new Dish()
                            {
                                Name = "2ForYou",
                                Price  = 6.99M,
                            },
                            new Dish()
                            {
                                Name = "Vanilla Shake",
                                Price = 10.50M,
                            }
                        },
                        Address = new Address()
                        {
                            City = "Warsaw",
                            Street = "Złota 54",
                            PostalCode = "00-002"
                        }
                    },
                };

            return restaurants;
        }
    }
}