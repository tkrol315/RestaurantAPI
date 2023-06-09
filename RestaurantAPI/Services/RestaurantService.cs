using AutoMapper;
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
using Microsoft.AspNetCore.Authorization;
using RestaurantAPI.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace RestaurantAPI.services
{
    public interface IRestaurantService
    {
        RestaurantDto GetById(int id);

        List<RestaurantDto> GetAll();

        int Create(CreateRestaurantDto dto, int userId);

        void Delete(int id, ClaimsPrincipal user);

        void Update(int id, UpdateRestaurantDto dto, ClaimsPrincipal user);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _authorizationService;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger
            , IAuthorizationService authorizationService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        public void Update(int id, UpdateRestaurantDto dto, ClaimsPrincipal user)
        {
            var restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.Id == id);
            if (restaurant == null) throw new NotFoundException("Restaurant not found");
            var authorizationResult = _authorizationService
                .AuthorizeAsync(user, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;
            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }
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

        public void Delete(int id, ClaimsPrincipal user)
        {
            _logger.LogWarning($"Action DELETE was invoked for Restaurant with id: {id}");
            var restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.Id == id);
            if (restaurant == null) throw new NotFoundException("Restaurant not found");
            var authorizationResult = _authorizationService
                .AuthorizeAsync(user, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;
            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }
            _dbContext.Remove(restaurant);
            _dbContext.SaveChanges();
        }

        public List<RestaurantDto> GetAll()
        {
            var restuarants = _dbContext.Restaurants.Include(r => r.Address).Include(r => r.Dishes).ToList();
            var restuarantDtos = _mapper.Map<List<RestaurantDto>>(restuarants);
            return restuarantDtos;
        }

        public int Create(CreateRestaurantDto dto, int userId)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = userId;
            _dbContext.Add(restaurant);
            _dbContext.SaveChanges();
            return restaurant.Id;
        }
    }
}