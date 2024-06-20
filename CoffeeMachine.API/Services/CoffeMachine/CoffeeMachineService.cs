using CoffeeMachine.API.Results;
using CoffeeMachine.API.Services.CoffeMachine.Utilities;
using CoffeeMachine.API.Services.Weather;
using Microsoft.Extensions.Options;

namespace CoffeeMachine.API.Services.CoffeMachine
{
    public class CoffeeMachineService : ICoffeeMachineService
    {
        private readonly ICallCounter _callCounter;
        private readonly IWeatherService _weatherService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly CoffeeMachineSettings _settings;
        private readonly ICoffeeMachineSettingsValidator _settingsValidator;

        public CoffeeMachineService(
            IWeatherService weatherService,
            IDateTimeProvider dateTimeProvider,
            ICallCounter callCounter,
            ICoffeeMachineSettingsValidator settingsValidator,
            IOptions<CoffeeMachineSettings> settings)
        {
            //constructor validation to prevent null injection
            _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _callCounter = callCounter ?? throw new ArgumentNullException(nameof(callCounter));
            _settingsValidator = settingsValidator ?? throw new ArgumentNullException(nameof(settingsValidator));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

            // Validate settings upon construction
            _settingsValidator.ValidateSettings(_settings);
        }

        public CoffeeMachineResponse MakeCoffee()
        {
            // Increment the call count atomically
            var callCount = _callCounter.Increment();

            // Check if it's the special date
            if (IsSpecialDate())
            {
                return new CoffeeMachineResponse { StatusCode = SpecalHttpCodes.ImATeapot };
            }

            // Check if it's every special number call
            if (IsSpecialNumberCall(callCount))
            {
                _callCounter.Reset();
                return new CoffeeMachineResponse { StatusCode = SpecalHttpCodes.ServiceUnavailable };
            }

            // Determine the message based on weather condition
            string message = _weatherService.IsHotWeather() ? _settings.HotWeatherMessage : _settings.NormalWeatherMessage;

            return new CoffeeMachineResponse
            {
                Message = message,
                StatusCode = SpecalHttpCodes.OK,
                Prepared = _dateTimeProvider.UtcNow.ToString(_settings.DateTimeFormatDefault)
            };
        }

        private bool IsSpecialDate()
        {
            var today = _dateTimeProvider.Today;
            return today.Month == _settings.SpecialDateMonth && today.Day == _settings.SpecialDateDay;
        }

        private bool IsSpecialNumberCall(int callCount)
        {
            return callCount % _settings.EverySpecialNumber == 0;
        }
    }
}
