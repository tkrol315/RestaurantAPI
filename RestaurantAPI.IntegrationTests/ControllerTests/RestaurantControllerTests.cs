using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using RestaurantAPI.Entities;
using RestaurantAPI.IntegrationTests.Helpers;
using RestaurantAPI.Models;
using System.Text;

namespace RestaurantAPI.IntegrationTests
{
    public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private HttpClient _httpClient;
        private WebApplicationFactory<Program> _factory;
        public RestaurantControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));
                        services.Remove(dbContextOptions);
                        services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));

                        services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                        services.AddMvc(options => options.Filters.Add(new FakeUserFIlter()));
                    });
                });
            _httpClient = _factory.CreateClient();
               
        }
        private void  SeedRestaurant(Restaurant restaurant)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<RestaurantDbContext>();
            dbContext.Restaurants.Add(restaurant);
            dbContext.SaveChanges();

        }
        [Theory]
        [InlineData("pageSize=5&pageNumber=1")]
        [InlineData("pageSize=10&pageNumber=2")]
        [InlineData("pageSize=15&pageNumber=3")]
        public async Task GetAll_WithQueryParameters_ReturnsOkResult(string queryParams)
        {
            var response = await _httpClient.GetAsync("/api/restaurant?" + queryParams);
          
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
        [Theory]
        [InlineData("pageSize=13&pageNumber=1")]
        [InlineData("pageSize=100&pageNumber=2")]
        [InlineData("pageSize=-1&pageNumber=3")]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetAll_WithInvalidQueryParams_ReturnsBadRequest(string queryParams)
        {
            var response = await _httpClient.GetAsync("/api/restaurant?" + queryParams);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task CreateRestaurant_WithValidModel_ReturnsCreatedStatus()
         {
            var dto = new CreateRestaurantDto()
            {
                Name = "TestRestaurant",
                ContactEmail = "TestEmail@Email.com",
                ContactNumber = "123456789",
                City = "TestCity",
                Street = "TestStreet"
            };

            var httpContent = dto.ToJsonHttpContent();

            var response = await _httpClient.PostAsync("/api/restaurant", httpContent);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }
        [Fact]
        public async Task CreateRestaurant_WithInvalidModel_ReturnsBadRequest()
        {
            var dto = new CreateRestaurantDto()
            {
                Description = "Test",
                PostalCode = "43-300",
                ContactEmail = "TestEmail@Email.com",
                ContactNumber = "123456789"
            };
            var httpContent = dto.ToJsonHttpContent();

            var response = await _httpClient.PostAsync("/api/restaurant",httpContent);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task Delete_ForNonExistingRestaurant_ReturnsNotFound()
        {
            var response = await _httpClient.DeleteAsync("/api/restaurant/" + 999);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
        [Fact]
        public async Task Delete_ForExistingRestaurant_ReturnsNoContent()
        {
            //arrange
            var restaurant = new Restaurant()
            {
                CreatedById = 1,
                Name = "Test",
                ContactEmail = "Test@Test",
                ContactNumber = "123456789"

            };
            SeedRestaurant(restaurant);

            //act
            var response = await _httpClient.DeleteAsync("/api/restaurant/" + restaurant.Id);
            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }
        [Fact]
        public async Task Delete_ForNonRestaurantOwner_ReturnsForbidden()
        {
            //arrange
            var restaurant = new Restaurant()
            {
                CreatedById = 999,
                Name = "Test",
                ContactEmail = "Test@Test",
                ContactNumber = "123456789"

            };
            SeedRestaurant(restaurant);
            //act
            var response = await _httpClient.DeleteAsync("/api/restaurant/" + restaurant.Id);
            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

    }
}
