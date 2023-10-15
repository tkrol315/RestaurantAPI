using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using RestaurantAPI.Controllers;
using RestaurantAPI.Services;

namespace RestaurantAPI.IntegrationTests
{
    public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly List<Type> _controllerTypes;
        public ProgramTests(WebApplicationFactory<Program> factory) 
        {
            _controllerTypes = typeof(Program).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ControllerBase))).ToList();
            _factory = factory
                .WithWebHostBuilder(builder => builder
                .ConfigureServices(services => _controllerTypes
                .ForEach(ct => services.AddScoped(ct))) 
            );
        }

        [Fact]
        public void Configure_Services_ForControllers_RegistersAllDependencies()
        {
            var factoryScope = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = factoryScope.CreateScope();

            _controllerTypes.ForEach(t =>
            {
                var controller = scope.ServiceProvider.GetService(t);
                controller.Should().NotBeNull();
            });
        }

    }
}
