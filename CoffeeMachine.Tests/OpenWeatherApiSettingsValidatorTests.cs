using CoffeeMachine.API.Services.Weather;
using FluentAssertions;

namespace CoffeeMachine.Tests
{

    public class OpenWeatherApiSettingsValidatorTests
    {
        private OpenWeatherApiSettingsValidator _validator;

        public OpenWeatherApiSettingsValidatorTests()
        {
            _validator = new OpenWeatherApiSettingsValidator();
        }

        [Fact]
        public void ValidateSettings_NullSettings_ThrowsArgumentNullException()
        {
            // Arrange
            OpenWheatherApiSettings settings = null;

            // Act & Assert
            Action act = () => _validator.ValidateSettings(settings);
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("openWheatherApiSettings");
        }

        [Fact]
        public void ValidateSettings_EmptyApiKey_ThrowsArgumentException()
        {
            // Arrange
            var settings = new OpenWheatherApiSettings
            {
                ApiKey = "", // Empty string
                TargetCity = "New York",
                HotTemperatureDefinition = 25
            };

            // Act & Assert
            Action act = () => _validator.ValidateSettings(settings);
            act.Should().Throw<ArgumentException>()
                .WithMessage($"{nameof(settings.ApiKey)} must not be empty");
        }

        [Fact]
        public void ValidateSettings_EmptyTargetCity_ThrowsArgumentException()
        {
            // Arrange
            var settings = new OpenWheatherApiSettings
            {
                ApiKey = "My_KEY",
                TargetCity = "", // Empty string
                HotTemperatureDefinition = 25
            };

            // Act & Assert
            Action act = () => _validator.ValidateSettings(settings);
            act.Should().Throw<ArgumentException>()
                .WithMessage($"{nameof(settings.TargetCity)} must not be empty");
        }

        [Fact]
        public void ValidateSettings_NegativeHotTemperatureDefinition_ThrowsArgumentException()
        {
            // Arrange
            var settings = new OpenWheatherApiSettings
            {
                ApiKey = "My_KEY",
                TargetCity = "New York",
                HotTemperatureDefinition = -5 // Negative value
            };

            // Act & Assert
            Action act = () => _validator.ValidateSettings(settings);
            act.Should().Throw<ArgumentException>()
                .WithMessage($"{nameof(settings.HotTemperatureDefinition)} must be greater than 0");
        }

        [Fact]
        public void ValidateSettings_ValidSettings_DoesNotThrow()
        {
            // Arrange
            var settings = new OpenWheatherApiSettings
            {
                ApiKey = "My_KEY",
                TargetCity = "New York",
                HotTemperatureDefinition = 25,
                OpenWheatherApiBaseUrl="bla"
            };

            // Act & Assert
            Action act = () => _validator.ValidateSettings(settings);
            act.Should().NotThrow();
        }
    }
}
