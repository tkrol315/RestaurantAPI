using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moq;
using RestaurantAPI.Entities;
using RestaurantAPI.IntegrationTests.Helpers;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.IntegrationTests.ControllerTests
{
    public class AccountControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private HttpClient _httpClient;
        private Mock<IAccountService> _accountServiceMock = new Mock<IAccountService>();
        public AccountControllerTests(WebApplicationFactory<Program> factory)
        {
            _httpClient = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContext = services.FirstOrDefault(s => s.ServiceType == typeof(RestaurantDbContext));
                    services.Remove(dbContext);
                    services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));
                    services.AddSingleton(_accountServiceMock.Object);
                });
            })
                .CreateClient();
        }

        [Fact]
        public async Task RegisterAccount_ForValidModel_ReturnsOk()
        {
            var dto = new CreateUserDto()
            {
                Email = $"TestEmail{DateTime.Now}@Test1.com",
                Password = "TestPassword123",
                ConfirmPassword = "TestPassword123"
            };

            var httpContent = dto.ToJsonHttpContent();

            var response = await _httpClient.PostAsync("/api/account/register", httpContent);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        }

        [Fact]
        public async Task RegisterAccount_ForInvalidModel_ReturnsBadRequest()
        {
            var dto = new CreateUserDto()
            {
                Password = "Test",
                ConfirmPassword = "TestPassword123"
            };
            var httpContent = dto.ToJsonHttpContent();

            var response = await _httpClient.PostAsync("/api/account/register", httpContent);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task Login_ForRegisteredUser_ReturnsOk()
        {
            _accountServiceMock.Setup(e => e.GenerateJwt(It.IsAny<LoginUserDto>())).Returns("jwt");
            var loginDto = new LoginUserDto()
            {
                Email = "Test@Test.com",
                Password = "Test",
            };
            var httpContent = loginDto.ToJsonHttpContent();
            var response = await _httpClient.PostAsync("/api/account/login", httpContent);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        }
    }
}
