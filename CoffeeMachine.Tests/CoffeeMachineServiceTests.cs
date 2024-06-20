using CoffeeMachine.API.Results;
using CoffeeMachine.API.Services.CoffeMachine;
using CoffeeMachine.API.Services.CoffeMachine.Utilities;
using CoffeeMachine.API.Services.Weather;
using FluentAssertions;
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

        public CoffeeMachineServiceTests()
        {
            _mockWeatherService = new Mock<IWeatherService>();
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();
            _mockCallCounter = new Mock<ICallCounter>();
            _mockSettingsValidator = new Mock<ICoffeeMachineSettingsValidator>();

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
                mockSettings);

            // Setup default validation behavior
            _mockSettingsValidator.Setup(sv => sv.ValidateSettings(It.IsAny<CoffeeMachineSettings>()));
        }

        [Fact]
        public void MakeCoffee_ReturnsImATeapot_OnSpecialDate()
        {
            // Arrange
            _mockDateTimeProvider.Setup(dp => dp.Today).Returns(new DateTime(2023, 4, 1));

            // Act
            var result = _coffeeMachineService.MakeCoffee();

            // Assert
            result.StatusCode.Should().Be(SpecalHttpCodes.ImATeapot);
        }

        [Fact]
        public void MakeCoffee_ReturnsServiceUnavailable_OnEverySpecialNumberCall()
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
                results.Add(_coffeeMachineService.MakeCoffee());
            }

            // Assert
            results[4].StatusCode.Should().Be(SpecalHttpCodes.ServiceUnavailable);
        }

        [Fact]
        public void MakeCoffee_ReturnsOk_OnRegularCall()
        {
            // Arrange
            _mockDateTimeProvider.Setup(dp => dp.Today).Returns(new DateTime(2023, 3, 31));
            _mockCallCounter.Setup(cc => cc.Increment()).Returns(1);
            _mockWeatherService.Setup(ws => ws.IsHotWeather()).Returns(false);

            // Act
            var result = _coffeeMachineService.MakeCoffee();

            // Assert
            result.StatusCode.Should().Be(SpecalHttpCodes.OK);
            result.Message.Should().Be("Your piping hot coffee is ready");
        }

        [Fact]
        public void MakeCoffee_ReturnsIcedCoffeeMessage_WhenHotWeather()
        {
            // Arrange
            _mockDateTimeProvider.Setup(dp => dp.Today).Returns(new DateTime(2023, 3, 31));
            _mockCallCounter.Setup(cc => cc.Increment()).Returns(1);
            _mockWeatherService.Setup(ws => ws.IsHotWeather()).Returns(true);

            // Act
            var result = _coffeeMachineService.MakeCoffee();

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
            Action act = () => new CoffeeMachineService(nullWeatherService, mockDateTimeProvider.Object, mockCallCounter.Object, _mockSettingsValidator.Object, mockSettings);

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
            Action act = () => new CoffeeMachineService(mockWeatherService.Object, nullDateTimeProvider, mockCallCounter.Object, _mockSettingsValidator.Object, mockSettings);

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
            Action act = () => new CoffeeMachineService(mockWeatherService.Object, mockDateTimeProvider.Object, nullCallCounter, _mockSettingsValidator.Object, mockSettings);

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
            Action act = () => new CoffeeMachineService(mockWeatherService.Object, mockDateTimeProvider.Object, mockCallCounter.Object, _mockSettingsValidator.Object, nullSettings);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'settings')");
        }

        [Fact]
        public void MakeCoffee_ReturnsImATeapot_OnCustomSpecial_Christmas_Date()
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
            var service = new CoffeeMachineService(_mockWeatherService.Object, _mockDateTimeProvider.Object, _mockCallCounter.Object, _mockSettingsValidator.Object, mockSettings);

            _mockDateTimeProvider.Setup(dp => dp.Today).Returns(new DateTime(2023, customSettings.SpecialDateMonth, customSettings.SpecialDateDay));

            // Act
            var result = service.MakeCoffee();

            // Assert
            result.StatusCode.Should().Be(SpecalHttpCodes.ImATeapot);
        }

        [Fact]
        public void MakeCoffee_ReturnsOk_WithCustomHotWeatherMessage()
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
            var service = new CoffeeMachineService(_mockWeatherService.Object, _mockDateTimeProvider.Object, _mockCallCounter.Object, _mockSettingsValidator.Object, mockSettings);

            _mockDateTimeProvider.Setup(dp => dp.Today).Returns(new DateTime(2023, 3, 31));
            _mockCallCounter.Setup(cc => cc.Increment()).Returns(1);
            _mockWeatherService.Setup(ws => ws.IsHotWeather()).Returns(true);

            // Act
            var result = service.MakeCoffee();

            // Assert
            result.StatusCode.Should().Be(SpecalHttpCodes.OK);
            result.Message.Should().Be(customSettings.HotWeatherMessage);
        }

        [Fact]
        public void MakeCoffee_ReturnsOk_WithCustomNormalWeatherMessage()
        {
            // Arrange
            var customSettings = new CoffeeMachineSettings
            {
                EverySpecialNumber = 5,
                SpecialDateMonth = 4,
                SpecialDateDay = 1,
                HotWeatherMessage = "Your refreshing iced coffee is ready",
                NormalWeatherMessage = "No coffee, but where is a planty of hot water!",
                DateTimeFormatDefault = "yyyy-MM-ddTHH:mm:sszzz"
            };
            var mockSettings = Options.Create(customSettings);
            var service = new CoffeeMachineService(_mockWeatherService.Object, _mockDateTimeProvider.Object, _mockCallCounter.Object, _mockSettingsValidator.Object, mockSettings);

            _mockDateTimeProvider.Setup(dp => dp.Today).Returns(new DateTime(2024, 3, 25));
            _mockCallCounter.Setup(cc => cc.Increment()).Returns(1);
            _mockWeatherService.Setup(ws => ws.IsHotWeather()).Returns(false);

            // Act
            var result = service.MakeCoffee();

            // Assert
            result.StatusCode.Should().Be(SpecalHttpCodes.OK);
            result.Message.Should().Be(customSettings.NormalWeatherMessage);
        }
    }
}
