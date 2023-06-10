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
using RestaurantAPI.Services;
using System;
using System.Linq.Expressions;

namespace RestaurantAPI.services
{
    public interface IRestaurantService
    {
        RestaurantDto GetById(int id);

        PagedResult<RestaurantDto> GetAll(RestaurantQuery query);

        int Create(CreateRestaurantDto dto);

        void Delete(int id);

        void Update(int id, UpdateRestaurantDto dto);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger
            , IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public void Update(int id, UpdateRestaurantDto dto)
        {
            var restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.Id == id);
            if (restaurant == null) throw new NotFoundException("Restaurant not found");
            var authorizationResult = _authorizationService
                .AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;
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

        public void Delete(int id)
        {
            _logger.LogWarning($"Action DELETE was invoked for Restaurant with id: {id}");
            var restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.Id == id);
            if (restaurant == null) throw new NotFoundException("Restaurant not found");
            var authorizationResult = _authorizationService
                .AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;
            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }
            _dbContext.Remove(restaurant);
            _dbContext.SaveChanges();
        }

        public PagedResult<RestaurantDto> GetAll(RestaurantQuery query)
        {
            var restaurants = _dbContext.Restaurants.Include(r => r.Address).Include(r => r.Dishes)
                .Where(r => query.SearchPhrase == null ||
                (
                r.Name.ToLower().Contains(query.SearchPhrase.ToLower())) ||
                r.Description.ToLower().Contains(query.SearchPhrase.ToLower())
                );

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columns = new Dictionary<string, Expression<Func<Restaurant, object>>>() {
                    {nameof(Restaurant.Name),r=>r.Name },
                    {nameof(Restaurant.Description),r=>r.Description },
                    {nameof(Restaurant.Category),r=>r.Category }
                };

                var selectedColumn = columns[query.SortBy];
                restaurants = query.SortDirection == SortDirection.ASC
                    ? restaurants.OrderBy(selectedColumn)
                    : restaurants.OrderByDescending(selectedColumn);
            }

            var selectedRestaurants = restaurants
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize).ToList();

            var totalItemsCount = restaurants.Count();
            var restuarantDtos = _mapper.Map<List<RestaurantDto>>(selectedRestaurants);
            var pagedResult = new PagedResult<RestaurantDto>(restuarantDtos, totalItemsCount, query.PageSize, query.PageNumber);
            return pagedResult;
        }

        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;
            _dbContext.Add(restaurant);
            _dbContext.SaveChanges();
            return restaurant.Id;
        }
    }
}