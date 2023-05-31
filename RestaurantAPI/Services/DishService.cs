using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System.Linq;

namespace RestaurantAPI.Services
{
    public interface IDishService
    {
        int Create(int restaurantId, CreateDishDto dto);
    }

    public class DishService : IDishService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;

        public DishService(RestaurantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int Create(int restaurantId, CreateDishDto dto)
        {
            var restaurant = _dbContext.Restaurants.Include(r => r.Dishes).FirstOrDefault(r => r.Id == restaurantId);
            if (restaurant == null) throw new NotFoundException($"Restaurant with id: {restaurantId} not found");
            var dish = _mapper.Map<Dish>(dto);
            dish.RestaurantId = restaurantId;
            restaurant.Dishes.Add(dish);
            _dbContext.SaveChanges();
            return dish.Id;
        }
    }
}