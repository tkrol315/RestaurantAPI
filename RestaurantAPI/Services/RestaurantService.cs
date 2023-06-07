﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using RestaurantAPI.Exceptions;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace RestaurantAPI.services
{
    public interface IRestaurantService
    {
        RestaurantDto GetById(int id);

        List<RestaurantDto> GetAll();

        int Create(CreateRestaurantDto dto);

        void Delete(int id);

        void Update(int id, UpdateRestaurantDto dto);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public void Update(int id, UpdateRestaurantDto dto)
        {
            var restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.Id == id);
            if (restaurant == null) throw new NotFoundException("Restaurant not found");
            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;
            _dbContext.SaveChanges();
        }

        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbContext.Restaurants.Include(r => r.Address).Include(r => r.Dishes).FirstOrDefault(r => r.Id == id);
            if (restaurant == null) throw new NotFoundException("Restaurant not found ");
            var result = _mapper.Map<RestaurantDto>(restaurant);
            return result;
        }

        public void Delete(int id)
        {
            _logger.LogWarning($"Action DELETE was invoked for Restaurant with id: {id}");
            var restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.Id == id);
            if (restaurant == null) throw new NotFoundException("Restaurant not found");
            _dbContext.Remove(restaurant);
            _dbContext.SaveChanges();
        }

        public List<RestaurantDto> GetAll()
        {
            var restuarants = _dbContext.Restaurants.Include(r => r.Address).Include(r => r.Dishes).ToList();
            var restuarantDtos = _mapper.Map<List<RestaurantDto>>(restuarants);
            return restuarantDtos;
        }

        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            _dbContext.Add(restaurant);
            _dbContext.SaveChanges();
            return restaurant.Id;
        }
    }
}