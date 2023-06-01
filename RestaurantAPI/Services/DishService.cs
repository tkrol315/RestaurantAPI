using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace RestaurantAPI.Services
{
    public interface IDishService
    {
        int Create(int restaurantId, CreateDishDto dto);

        DishDto GetById(int restaurantId, int dishId);

        List<DishDto> GetAll(int restaurantId);

        void RemoveById(int restaurantId, int dishId);

        void RemoveAll(int restaurantId);
    }

    public class DishService : IDishService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;

        private Restaurant GetRestaurantById(int id)
        {
            var restaurant = _dbContext.Restaurants.Include(r => r.Dishes).FirstOrDefault(r => r.Id == id);
            if (restaurant == null) throw new NotFoundException($"Restaurant not found");
            return restaurant;
        }

        public DishService(RestaurantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int Create(int restaurantId, CreateDishDto dto)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dish = _mapper.Map<Dish>(dto);
            dish.RestaurantId = restaurantId;
            restaurant.Dishes.Add(dish);
            _dbContext.SaveChanges();
            return dish.Id;
        }

        public DishDto GetById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dish = restaurant.Dishes.FirstOrDefault(d => d.Id == dishId);
            if (dish == null) throw new NotFoundException($"Dish not found");
            var dishDto = _mapper.Map<DishDto>(dish);
            return dishDto;
        }

        public List<DishDto> GetAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dishesDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes);
            return dishesDtos;
        }

        public void RemoveById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dish = restaurant.Dishes.FirstOrDefault(d => d.Id == dishId);
            if (dish == null) throw new NotFoundException("Dish not found");
            restaurant.Dishes.Remove(dish);
            _dbContext.SaveChanges();
        }

        public void RemoveAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            _dbContext.RemoveRange(restaurant.Dishes);
            _dbContext.SaveChanges();
        }
    }
}