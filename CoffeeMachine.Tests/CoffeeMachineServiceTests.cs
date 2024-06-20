using CoffeeMachine.API.Results;
using CoffeeMachine.API.Services.CoffeMachine;
using CoffeeMachine.API.Services.CoffeMachine.Utilities;
using CoffeeMachine.API.Services.Weather;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;


namespace CoffeeMachine.Tests
{

    public class CoffeeMachineServiceTests
    {
        private readonly Mock<IWeatherService> _mockWeatherService;
        private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
        private readonly Mock<ICallCounter> _mockCallCounter;
        private readonly Mock<ICoffeeMachineSettingsValidator> _mockSettingsValidator;
        private readonly CoffeeMachineService _coffeeMachineService;
        private readonly Mock<ILogger<ICoffeeMachineService>> _mockLogger;

        public CoffeeMachineServiceTests()
        {
            _mockWeatherService = new Mock<IWeatherService>();
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();
            _mockCallCounter = new Mock<ICallCounter>();
            _mockSettingsValidator = new Mock<ICoffeeMachineSettingsValidator>();
            _mockLogger = new Mock<ILogger<ICoffeeMachineService>>();

            var coffeeMachineSettings = new CoffeeMachineSettings
            {
                EverySpecialNumber = 5,
                SpecialDateMonth = 4,
                SpecialDateDay = 1,
                HotWeatherMessage = "Your refreshing iced coffee is ready",
                NormalWeatherMessage = "Your piping hot coffee is ready",
                DateTimeFormatDefault = "yyyy-MM-ddTHH:mm:sszzz"
            };

            var mockSettings = Options.Create(coffeeMachineSettings);

            _coffeeMachineService = new CoffeeMachineService(
                _mockWeatherService.Object,
                _mockDateTimeProvider.Object,
                _mockCallCounter.Object,
                _mockSettingsValidator.Object,
                mockSettings,
                _mockLogger.Object);

            // Setup default validation behavior
            _mockSettingsValidator.Setup(sv => sv.ValidateSettings(It.IsAny<CoffeeMachineSettings>()));
        }

        [Fact]
        public async Task MakeCoffee_ReturnsImATeapot_OnSpecialDate()
        {
            // Arrange
            _mockDateTimeProvider.Setup(dp => dp.Today).Returns(new DateTime(2023, 4, 1));

            // Act
            var result = await _coffeeMachineService.MakeCoffee();

            // Assert
            result.StatusCode.Should().Be(SpecalHttpCodes.ImATeapot);
        }

        [Fact]
        public async Task MakeCoffee_ReturnsServiceUnavailable_OnEverySpecialNumberCall()
        {
            // Arrange
            _mockCallCounter.SetupSequence(cc => cc.Increment())
                .Returns(1)
                .Returns(2)
                .Returns(3)
                .Returns(4)
                .Returns(5); // Fifth call

            // Act
            var results = new List<CoffeeMachineResponse>();
            for (int i = 0; i < 5; i++)
            {
                results.Add(await _coffeeMachineService.MakeCoffee());
            }

            // Assert
            results[4].StatusCode.Should().Be(SpecalHttpCodes.ServiceUnavailable);
        }

        [Fact]
        public async Task MakeCoffee_ReturnsOk_OnRegularCall()
        {
            // Arrange
            _mockDateTimeProvider.Setup(dp => dp.Today).Returns(new DateTime(2023, 3, 31));
            _mockCallCounter.Setup(cc => cc.Increment()).Returns(1);
            _mockWeatherService.Setup(ws => ws.IsHotWeatherAsync()).ReturnsAsync(false);

            // Act
            var result = await _coffeeMachineService.MakeCoffee();

            // Assert
            result.StatusCode.Should().Be(SpecalHttpCodes.OK);
            result.Message.Should().Be("Your piping hot coffee is ready");
        }

        [Fact]
        public async Task MakeCoffee_ReturnsIcedCoffeeMessage_WhenHotWeather()
        {
            // Arrange
            _mockDateTimeProvider.Setup(dp => dp.Today).Returns(new DateTime(2023, 3, 31));
            _mockCallCounter.Setup(cc => cc.Increment()).Returns(1);
            _mockWeatherService.Setup(ws => ws.IsHotWeatherAsync()).ReturnsAsync(true);

            // Act
            var result = await _coffeeMachineService.MakeCoffee();

            // Assert
            result.StatusCode.Should().Be(SpecalHttpCodes.OK);
            result.Message.Should().Be("Your refreshing iced coffee is ready");
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenWeatherServiceIsNull()
        {
            // Arrange
            IWeatherService nullWeatherService = null;
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            var mockCallCounter = new Mock<ICallCounter>();
            var mockSettings = Options.Create(new CoffeeMachineSettings());

            // Act
            Action act = () => new CoffeeMachineService(nullWeatherService, mockDateTimeProvider.Object, mockCallCounter.Object, _mockSettingsValidator.Object, mockSettings,
                _mockLogger.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'weatherService')");
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenDateTimeProviderIsNull()
        {
            // Arrange
            var mockWeatherService = new Mock<IWeatherService>();
            IDateTimeProvider nullDateTimeProvider = null;
            var mockCallCounter = new Mock<ICallCounter>();
            var mockSettings = Options.Create(new CoffeeMachineSettings());

            // Act
            Action act = () => new CoffeeMachineService(mockWeatherService.Object, nullDateTimeProvider, mockCallCounter.Object, _mockSettingsValidator.Object, mockSettings,
                _mockLogger.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'dateTimeProvider')");
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenCallCounterIsNull()
        {
            // Arrange
            var mockWeatherService = new Mock<IWeatherService>();
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            ICallCounter nullCallCounter = null;
            var mockSettings = Options.Create(new CoffeeMachineSettings());

            // Act
            Action act = () => new CoffeeMachineService(mockWeatherService.Object, mockDateTimeProvider.Object, nullCallCounter, _mockSettingsValidator.Object, mockSettings,
                _mockLogger.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'callCounter')");
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenSettingsIsNull()
        {
            // Arrange
            var mockWeatherService = new Mock<IWeatherService>();
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            var mockCallCounter = new Mock<ICallCounter>();
            IOptions<CoffeeMachineSettings> nullSettings = null;

            // Act
            Action act = () => new CoffeeMachineService(mockWeatherService.Object, mockDateTimeProvider.Object, mockCallCounter.Object, _mockSettingsValidator.Object, nullSettings,
                _mockLogger.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'settings')");
        }

        [Fact]
        public async Task MakeCoffee_ReturnsImATeapot_OnCustomSpecial_Christmas_Date()
        {
            // Arrange
            var customSettings = new CoffeeMachineSettings
            {
                EverySpecialNumber = 5,
                SpecialDateMonth = 12,
                SpecialDateDay = 25, // Christmas
                HotWeatherMessage = "Your refreshing iced coffee is ready",
                NormalWeatherMessage = "Your piping hot coffee is ready",
                DateTimeFormatDefault = "yyyy-MM-ddTHH:mm:sszzz"
            };
            var mockSettings = Options.Create(customSettings);
            var service = new CoffeeMachineService(_mockWeatherService.Object, _mockDateTimeProvider.Object, _mockCallCounter.Object, _mockSettingsValidator.Object, mockSettings, _mockLogger.Object);

            _mockDateTimeProvider.Setup(dp => dp.Today).Returns(new DateTime(2023, customSettings.SpecialDateMonth, customSettings.SpecialDateDay));

            // Act
            var result = await service.MakeCoffee();

            // Assert
            result.StatusCode.Should().Be(SpecalHttpCodes.ImATeapot);
        }

        [Fact]
        public async Task MakeCoffee_ReturnsOk_WithCustomHotWeatherMessage()
        {
            // Arrange
            var customSettings = new CoffeeMachineSettings
            {
                EverySpecialNumber = 5,
                SpecialDateMonth = 4,
                SpecialDateDay = 1,
                HotWeatherMessage = "No coffee today. Take cold water!",
                NormalWeatherMessage = "Your piping hot coffee is ready",
                DateTimeFormatDefault = "yyyy-MM-ddTHH:mm:sszzz"
            };
            var mockSettings = Options.Create(customSettings);
            var service = new CoffeeMachineService(_mockWeatherService.Object, _mockDateTimeProvider.Object, _mockCallCounter.Object, _mockSettingsValidator.Object, mockSettings, _mockLogger.Object);

            _mockDateTimeProvider.Setup(dp => dp.Today).Returns(new DateTime(2023, 3, 31));
            _mockCallCounter.Setup(cc => cc.Increment()).Returns(1);
            _mockWeatherService.Setup(ws => ws.IsHotWeatherAsync()).ReturnsAsync(true);

            // Act
            var result = await service.MakeCoffee();

            // Assert
            result.StatusCode.Should().Be(SpecalHttpCodes.OK);
            result.Message.Should().Be(customSettings.HotWeatherMessage);
        }

        [Fact]
        public async Task MakeCoffee_ReturnsOk_WithCustomNormalWeatherMessage()
        {
            // Arrange
            var customSettings = new CoffeeMachineSettings
            {
                EverySpecialNumber = 5,
                SpecialDateMonth = 4,
                SpecialDateDay = 1,
                HotWeatherMessage = "Your refreshing iced coffee is ready",
                NormalWeatherMessage = "No coffee, but there is plenty of hot water!",
                DateTimeFormatDefault = "yyyy-MM-ddTHH:mm:sszzz"
            };
            var mockSettings = Options.Create(customSettings);
            var service = new CoffeeMachineService(_mockWeatherService.Object, _mockDateTimeProvider.Object, _mockCallCounter.Object, _mockSettingsValidator.Object, mockSettings, _mockLogger.Object);

            _mockDateTimeProvider.Setup(dp => dp.Today).Returns(new DateTime(2024, 3, 25));
            _mockCallCounter.Setup(cc => cc.Increment()).Returns(1);
            _mockWeatherService.Setup(ws => ws.IsHotWeatherAsync()).ReturnsAsync(false);

            // Act
            var result = await service.MakeCoffee();

            // Assert
            result.StatusCode.Should().Be(SpecalHttpCodes.OK);
            result.Message.Should().Be(customSettings.NormalWeatherMessage);
        }

        [Fact]
        public async Task MakeCoffee_ReturnsOk_WithCustomSettings_SpecialDate()
        {
            // Arrange
            var customSettings = new CoffeeMachineSettings
            {
                EverySpecialNumber = 3,
                SpecialDateMonth = 6,
                SpecialDateDay = 15,
                HotWeatherMessage = "Enjoy your cold brew!",
                NormalWeatherMessage = "Enjoy your hot coffee!",
                DateTimeFormatDefault = "yyyy-MM-ddTHH:mm:sszzz"
            };
            var mockSettings = Options.Create(customSettings);
            var service = new CoffeeMachineService(_mockWeatherService.Object, _mockDateTimeProvider.Object, _mockCallCounter.Object, _mockSettingsValidator.Object, mockSettings, _mockLogger.Object);

            _mockDateTimeProvider.Setup(dp => dp.Today).Returns(new DateTime(2023, customSettings.SpecialDateMonth, customSettings.SpecialDateDay));

            // Act
            var result = await service.MakeCoffee();

            // Assert
            result.StatusCode.Should().Be(SpecalHttpCodes.ImATeapot);
        }

        [Fact]
        public async Task MakeCoffee_ReturnsServiceUnavailable_OnSpecialNumberCall_CustomSettings()
        {
            // Arrange
            var customSettings = new CoffeeMachineSettings
            {
                EverySpecialNumber = 3,
                SpecialDateMonth = 6,
                SpecialDateDay = 15,
                HotWeatherMessage = "Enjoy your cold brew!",
                NormalWeatherMessage = "Enjoy your hot coffee!",
                DateTimeFormatDefault = "yyyy-MM-ddTHH:mm:sszzz"
            };
            var mockSettings = Options.Create(customSettings);
            var service = new CoffeeMachineService(_mockWeatherService.Object, _mockDateTimeProvider.Object, _mockCallCounter.Object, _mockSettingsValidator.Object, mockSettings, _mockLogger.Object);

            _mockCallCounter.SetupSequence(cc => cc.Increment())
                .Returns(1)
                .Returns(2)
                .Returns(3); // Third call

            // Act
            var results = new List<CoffeeMachineResponse>();
            for (int i = 0; i < 3; i++)
            {
                results.Add(await service.MakeCoffee());
            }

            // Assert
            results[2].StatusCode.Should().Be(SpecalHttpCodes.ServiceUnavailable);
        }

        [Fact]
        public async Task MakeCoffee_ShouldReturnServiceUnavailable_WhenHttpRequestExceptionOccurs()
        {
            // Arrange
            _mockWeatherService.Setup(ws => ws.IsHotWeatherAsync())
                .ThrowsAsync(new HttpRequestException("Error communicating weather service"));
            var customSettings = new CoffeeMachineSettings
            {
                EverySpecialNumber = 3,
                SpecialDateMonth = 6,
                SpecialDateDay = 15,
                HotWeatherMessage = "Enjoy your cold brew!",
                NormalWeatherMessage = "Enjoy your hot coffee!",
                DateTimeFormatDefault = "yyyy-MM-ddTHH:mm:sszzz"
            };
            _mockCallCounter.SetupSequence(cc => cc.Increment())
                .Returns(1);
            var mockSettings = Options.Create(customSettings);
            var service = new CoffeeMachineService(_mockWeatherService.Object, _mockDateTimeProvider.Object, _mockCallCounter.Object, _mockSettingsValidator.Object, mockSettings, _mockLogger.Object);
            // Act
            var result = await service.MakeCoffee();

            // Assert
            Assert.Equal(SpecalHttpCodes.ServiceUnavailable, result.StatusCode);
            Assert.Equal(CoffeeMachineService.ErrorContactingWheatherServiceMessage, result.Message);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("HTTP request error in MakeCoffee")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
    }
}