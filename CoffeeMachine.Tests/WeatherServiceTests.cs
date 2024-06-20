using CoffeeMachine.API.Services.Weather;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;

namespace CoffeeMachine.Tests
{

    public class WeatherServiceTests
    {
        private readonly Mock<IWeatherHttpClient> _mockWeatherHttpClient;
        private readonly Mock<IOpenWeatherApiSettingsValidator> _mockValidator;
        private readonly Mock<ILogger<WeatherService>> _mockLogger;
        private readonly IOptions<OpenWheatherApiSettings> _options;
        private readonly WeatherService _weatherService;

        public WeatherServiceTests()
        {
            _mockWeatherHttpClient = new Mock<IWeatherHttpClient>();
            _mockValidator = new Mock<IOpenWeatherApiSettingsValidator>();
            _mockLogger = new Mock<ILogger<WeatherService>>();

            var settings = new OpenWheatherApiSettings
            {
                ApiKey = "test-api-key",
                OpenWheatherApiBaseUrl = "https://api.openweathermap.org/data/2.5/",
                TargetCity = "TestCity",
                HotTemperatureDefinition = 30
            };
            _options = Options.Create(settings);

            _weatherService = new WeatherService(_mockWeatherHttpClient.Object, _options, _mockValidator.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task IsHotWeatherAsync_ShouldReturnTrue_WhenTemperatureIsAboveThreshold()
        {
            // Arrange
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"main\":{\"temp\":35.0}}")
            };
            _mockWeatherHttpClient.Setup(client => client.GetWeatherDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var result = await _weatherService.IsHotWeatherAsync();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsHotWeatherAsync_ShouldReturnFalse_WhenTemperatureIsBelowThreshold()
        {
            // Arrange
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"main\":{\"temp\":25.0}}")
            };
            _mockWeatherHttpClient.Setup(client => client.GetWeatherDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var result = await _weatherService.IsHotWeatherAsync();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsHotWeatherAsync_ShouldLogWarning_WhenResponseIsUnsuccessful()
        {
            // Arrange
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            };
            _mockWeatherHttpClient.Setup(client => client.GetWeatherDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var result = await _weatherService.IsHotWeatherAsync();

            // Assert
            result.Should().BeFalse();
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Received unsuccessful response from OpenWeatherMap API")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task IsHotWeatherAsync_ShouldThrowException_WhenHttpRequestExceptionOccurs()
        {
            // Arrange
            _mockWeatherHttpClient.Setup(client => client.GetWeatherDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _weatherService.IsHotWeatherAsync());
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error calling OpenWeatherMap API")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task IsHotWeatherAsync_Returns_False_ForHotCity_with_High_HotTemperatureDefinition()
        {
            // Arrange
            var settings = new OpenWheatherApiSettings
            {
                ApiKey = "test-api-key",
                OpenWheatherApiBaseUrl = "https://api.openweathermap.org/data/2.5/",
                TargetCity = "VeryHotCity",
                HotTemperatureDefinition = 40// extra high hot temp definition
            };
            var optionSettings = Options.Create(settings);
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"main\":{\"temp\":39.0}}")// not hot enouth 
            };

            _mockWeatherHttpClient.Setup(client => client.GetWeatherDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            var weatherService = new WeatherService(_mockWeatherHttpClient.Object, optionSettings, _mockValidator.Object, _mockLogger.Object);

            // Act
            var result = await weatherService.IsHotWeatherAsync();


            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsHotWeatherAsync_ReturnsTrue_ForColdCity_with_low_HotTemperatureDefinition()
        {
            // Arrange

            var settings = new OpenWheatherApiSettings
            {
                ApiKey = "test-api-key",
                OpenWheatherApiBaseUrl = "https://api.openweathermap.org/data/2.5/",
                TargetCity = "ColdCity",
                HotTemperatureDefinition = 10// cold temp definition
            };
            var optionSettings = Options.Create(settings);
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"main\":{\"temp\":12.0}}")// cold, but hot for this city
            };

            _mockWeatherHttpClient.Setup(client => client.GetWeatherDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            var weatherService = new WeatherService(_mockWeatherHttpClient.Object, optionSettings, _mockValidator.Object, _mockLogger.Object);

            // Act
            var result = await weatherService.IsHotWeatherAsync();


            // Assert
            result.Should().BeTrue();
        }
    }
}
