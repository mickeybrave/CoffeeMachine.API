using CoffeeMachine.API.Services.CoffeMachine;
using CoffeeMachine.API.Services.CoffeMachine.Utilities;

namespace CoffeeMachine.Tests
{

    public class CoffeeMachineSettingsValidatorTests
    {
        private readonly ICoffeeMachineSettingsValidator _validator;

        public CoffeeMachineSettingsValidatorTests()
        {
            _validator = new CoffeeMachineSettingsValidator();
        }

        [Fact]
        public void ValidateSettings_ValidSettings_ShouldNotThrow()
        {
            // Arrange
            var settings = new CoffeeMachineSettings
            {
                EverySpecialNumber = 5,
                SpecialDateMonth = 12,
                SpecialDateDay = 25
            };

            // Act
            _validator.ValidateSettings(settings);

            // Assert: No exception should be thrown
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ValidateSettings_EverySpecialNumberInvalid_ThrowsArgumentException(int everySpecialNumber)
        {
            // Arrange
            var settings = new CoffeeMachineSettings
            {
                EverySpecialNumber = everySpecialNumber,
                SpecialDateMonth = 1,
                SpecialDateDay = 1
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _validator.ValidateSettings(settings));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(13)]
        public void ValidateSettings_SpecialDateMonthInvalid_ThrowsArgumentException(int specialDateMonth)
        {
            // Arrange
            var settings = new CoffeeMachineSettings
            {
                EverySpecialNumber = 1,
                SpecialDateMonth = specialDateMonth,
                SpecialDateDay = 1
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _validator.ValidateSettings(settings));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(32)]
        public void ValidateSettings_SpecialDateDayInvalid_ThrowsArgumentException(int specialDateDay)
        {
            // Arrange
            var settings = new CoffeeMachineSettings
            {
                EverySpecialNumber = 1,
                SpecialDateMonth = 1,
                SpecialDateDay = specialDateDay
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _validator.ValidateSettings(settings));
        }

        [Theory]
        [InlineData(30, 2)]
        [InlineData(31, 6)]
        [InlineData(31, 11)]
        public void ValidateSettings_SpecialDateDay_and_EverySpecialNumber_combination_Invalid_ThrowsArgumentException(int specialDateDay, int specialDateMonth)
        {
            // Arrange
            var settings = new CoffeeMachineSettings
            {
                EverySpecialNumber = 1,
                SpecialDateMonth = specialDateMonth,
                SpecialDateDay = specialDateDay
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _validator.ValidateSettings(settings));
        }

        [Theory]
        [InlineData(30, 4)]
        [InlineData(30, 9)]
        [InlineData(30, 11)]
        public void ValidateSettings_SpecialDateDay_and_EverySpecialNumber_combination_Valid_Not_ThrowsArgumentException(int specialDateDay, int specialDateMonth)
        {
            // Arrange
            var settings = new CoffeeMachineSettings
            {
                EverySpecialNumber = 1,
                SpecialDateMonth = specialDateMonth,
                SpecialDateDay = specialDateDay
            };

            // Act & Assert
            var exception = Record.Exception(() => _validator.ValidateSettings(settings));
            Assert.False(exception is ArgumentException, $"Expected no exception, but got {exception}");
        }
    }
}
