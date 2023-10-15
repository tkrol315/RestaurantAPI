using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NLog.Config;
using RestaurantAPI.Entities;
using RestaurantAPI.IntegrationTests.Helpers;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace RestaurantAPI.IntegrationTests.ControllerTests
{
    public class DishControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _httpClient;
        private RestaurantDbContext _dbContext;

        public DishControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = CreateHttpClient("InMemoryDatabase");
            var serviceScope = factory.Services.GetService<IServiceScopeFactory>().CreateScope();
            _dbContext = serviceScope.ServiceProvider.GetService<RestaurantDbContext>();
            SeedDishes();
        }

        private HttpClient CreateHttpClient(string databaseName)
        {
            var factory = new WebApplicationFactory<Program>();
            _factory = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));
                        services.Remove(dbContextOptions);
                        services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase(databaseName));
                    });
                });
            return _factory.CreateClient();

        }
        private void SeedDishes()
        {
            var dishList = new List<Dish>()
            {
                new Dish()
                {
                    Name = "TestDish1",
                    Price = 12.99M,
                    RestaurantId = 1
                },
                 new Dish()
                {
                    Name = "TestDish2",
                    Price = 17M,
                    RestaurantId = 1
                },
                new Dish()
                {
                    Name = "TestDish3",
                    Price = 15M,
                    RestaurantId = 2
                },
                 new Dish()
                {
                    Name = "TestDish4",
                    Price = 5.99M,
                    RestaurantId = 2
                }

            };
            _dbContext.AddRange(dishList);
            _dbContext.SaveChanges();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]

        public async Task GetAllDishes_ForValidRestaurantId_ReturnsOkResult(int restaurantId)
        {
            //arrange

            //act
            var response = await _httpClient.GetAsync($"/api/restaurant/{restaurantId}/dish/");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public async Task GetAllDishes_ForInvalidRestaurantId_ReturnsNotFound(int restaurantId)
        {

            var response = await _httpClient.GetAsync($"/api/restaurant/{restaurantId}/dish/");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 3)]
        public async Task GetDishById_ForValidRestaurantAndDishId_ReturnsOK(int restaurantId, int dishId)
        {
            //arrange

            //act
            var response = await _httpClient.GetAsync($"/api/restaurant/{restaurantId}/dish/{dishId}");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(1, -999)]
        [InlineData(2, 0)]
        public async Task GetDishById_ForInvalidDishId_ReturnsNotFound(int restaurantId, int dishId)
        {
            //arrange

            //act
            var response = await _httpClient.GetAsync($"/api/restaurant/{restaurantId}/dish/{dishId}");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task CreateDish_ForValidModel_ReturnsCreated(int restaurantId)
        {
            //arrange
            var dto = new Dish()
            {
                Name = "TestName",
                Price = 12.99M,
                RestaurantId = restaurantId
            };
            var httpContent = dto.ToJsonHttpContent();

            //act
            var response = await _httpClient.PostAsync($"/api/restaurant/{restaurantId}/dish", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task CreateDish_ForInvalidModel_ReturnsBadRequest(int restaurantId)
        {
            //arrange
            var dto = new Dish()
            {
                Name = "TestName",
            };

            var httpContent = dto.ToJsonHttpContent();

            //act
            var response = await _httpClient.PostAsync($"/api/restaurant/{restaurantId}/dish", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        public async Task Delete_ForValidDishId_ReturnsNoContent(int restaurantId, int dishId)
        {

            //arrange
            _httpClient = CreateHttpClient("InMemoryDatabaseForDelete1");
            //act
            var response = await _httpClient.DeleteAsync($"/api/restaurant/{restaurantId}/dish/{dishId}");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Theory]
        [InlineData(1, -999)]
        [InlineData(2, 0)]
        public async Task Delete_ForInvalidDishId_ReturnsNotFound(int restaurantId, int dishId)
        {

            //arrange

            //act
            var response = await _httpClient.DeleteAsync($"/api/restaurant/{restaurantId}/dish/{dishId}");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task DeleteAll_ForValidRestaurantId_ReturnsNoContent(int restaurantId)
        {
            //arrange
            _httpClient = CreateHttpClient("InMemoryDatabaseForDelete2");
            //act
            var response = await _httpClient.DeleteAsync($"/api/restaurant/{restaurantId}/dish/");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public async Task DeleteAll_ForInvalidRestaurantId_ReturnsNotFound(int restuarantId)
        {
            //arrange

            //act
            var response = await _httpClient.DeleteAsync($"/api/restaurant/{restuarantId}/dish/");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}
