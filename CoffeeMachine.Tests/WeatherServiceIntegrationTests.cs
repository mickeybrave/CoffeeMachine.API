using CoffeeMachine.API.Services.Weather;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;

namespace CoffeeMachine.Tests
{
    public class WeatherServiceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        private readonly IOptions<OpenWheatherApiSettings> _settings;

        public WeatherServiceIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _settings = Options.Create(new OpenWheatherApiSettings
            {
                ApiKey = "354e66351c1485fdbb4bdac77ed38e51",
                TargetCity = "Cebu",
                HotTemperatureDefinition = 20,
                OpenWheatherApiBaseUrl = "https://api.openweathermap.org/data/2.5/"
            });
        }

        [Fact]
        public async Task IsHotWeatherAsync_ReturnsTrueForHotCity()
        {
            // Arrange
            var openWeatherApiSettingsValidator = new OpenWeatherApiSettingsValidator();
            var logger = Mock.Of<ILogger<WeatherService>>();

            var weatherService = new WeatherService(
                new HttpClient(),
                _settings,
                openWeatherApiSettingsValidator,
                logger);

            // Act
            var isHot = await weatherService.IsHotWeatherAsync();

            // Assert
            isHot.Should().BeTrue();
        }

        [Fact]
        public async Task IsHotWeatherAsync_ReturnsTrue_ForHotCity_with_low_HotTemperatureDefinition()
        {
            // Arrange
            var openWeatherApiSettingsValidator = new OpenWeatherApiSettingsValidator();
            var logger = Mock.Of<ILogger<WeatherService>>();

            var weatherService = new WeatherService(
                new HttpClient(),
                _settings,
                openWeatherApiSettingsValidator,
                logger);

            // Act
            var isHot = await weatherService.IsHotWeatherAsync();

            // Assert
            isHot.Should().BeTrue();
        }

        [Fact]
        public async Task IsHotWeatherAsync_ReturnsFalseForCoolCity()
        {
            // Arrange
            var settings = Options.Create(new OpenWheatherApiSettings
            {
                ApiKey = "354e66351c1485fdbb4bdac77ed38e51",
                TargetCity = "New York",
                HotTemperatureDefinition = 25,
                OpenWheatherApiBaseUrl = "https://api.openweathermap.org/data/2.5/"
            });

            var openWeatherApiSettingsValidator = new OpenWeatherApiSettingsValidator();
            var logger = Mock.Of<ILogger<WeatherService>>();

            var weatherService = new WeatherService(
                new HttpClient(),
                settings,
                openWeatherApiSettingsValidator,
                logger);

            // Act
            var isHot = await weatherService.IsHotWeatherAsync();

            // Assert
            isHot.Should().BeFalse();
        }

        [Fact]
        public async Task IsHotWeatherAsync_ReturnsFalseForNonExistentCity()
        {
            // Arrange
            var settings = Options.Create(new OpenWheatherApiSettings
            {
                ApiKey = "354e66351c1485fdbb4bdac77ed38e51",
                TargetCity = "NonExistentCity",
                HotTemperatureDefinition = 25,
                OpenWheatherApiBaseUrl = "https://api.openweathermap.org/data/2.5/"
            });

            var openWeatherApiSettingsValidator = new OpenWeatherApiSettingsValidator();
            var logger = Mock.Of<ILogger<WeatherService>>();

            var weatherService = new WeatherService(
                new HttpClient(),
                settings,
                openWeatherApiSettingsValidator,
                logger);

            // Act
            var isHot = await weatherService.IsHotWeatherAsync();

            // Assert
            isHot.Should().BeFalse();
        }
    }
}
