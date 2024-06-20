using CoffeeMachine.API.Results;
using CoffeeMachine.API.Services.CoffeMachine;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace CoffeeMachine.Tests
{
    public class CoffeeMachineControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly CoffeeMachineSettings _settings;


        public CoffeeMachineControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            var configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")// adding real configuration into the integration tests to make sure that our configuration is valid. no nulls or 0
              .Build();
            _settings = new CoffeeMachineSettings();
            configuration.GetSection("CoffeeMachineSettings").Bind(_settings);
        }

        [Fact]
        public async Task MakeCoffee_ReturnsOkWithPipingHotCoffee()
        {
            // Arrange
            var mockService = new Mock<ICoffeeMachineService>();
            mockService.Setup(service => service.MakeCoffee())
                .Returns(new CoffeeMachineResponse
                {
                    StatusCode = SpecalHttpCodes.OK,
                    Message = "Your piping hot coffee is ready",
                    Prepared = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz")
                });


            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(Options.Create(_settings));
                    services.AddScoped(_ => mockService.Object);
                });
            }).CreateClient();


            // Act
            var response = await client.GetAsync("/brew-coffee");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Contain("Your piping hot coffee is ready");
        }

        [Fact]
        public async Task MakeCoffee_ReturnsImATeapot()
        {
            // Arrange
            var mockService = new Mock<ICoffeeMachineService>();
            mockService.Setup(service => service.MakeCoffee())
                .Returns(new CoffeeMachineResponse
                {
                    StatusCode = SpecalHttpCodes.ImATeapot
                });

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped(_ => mockService.Object);
                    services.AddSingleton(Options.Create(_settings));
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/brew-coffee");

            // Assert
            response.StatusCode.Should().Be((System.Net.HttpStatusCode)SpecalHttpCodes.ImATeapot);
        }

        [Fact]
        public async Task MakeCoffee_ReturnsServiceUnavailable()
        {
            // Arrange
            var mockService = new Mock<ICoffeeMachineService>();
            mockService.Setup(service => service.MakeCoffee())
                .Returns(new CoffeeMachineResponse
                {
                    StatusCode = SpecalHttpCodes.ServiceUnavailable
                });

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped(_ => mockService.Object);
                    services.AddSingleton(Options.Create(_settings));
                });
            }).CreateClient();


            // Act
            var response = await client.GetAsync("/brew-coffee");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.ServiceUnavailable);
        }
    }

}
