using CoffeeMachine.API.Results;
using CoffeeMachine.API.Services.CoffeMachine.Utilities;
using CoffeeMachine.API.Services.Weather;
using Microsoft.Extensions.Options;

namespace CoffeeMachine.API.Services.CoffeMachine
{
    public class CoffeeMachineService : ICoffeeMachineService
    {
        public const string ErrorContactingWheatherServiceMessage = "There was an error contacting the weather service.";
        public const string ErrorUnexpectedMessage = "An unexpected error occurred while preparing your coffee.";

        private readonly ICallCounter _callCounter;
        private readonly IWeatherService _weatherService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly CoffeeMachineSettings _settings;
        private readonly ICoffeeMachineSettingsValidator _settingsValidator;
        private readonly ILogger<ICoffeeMachineService> _logger;

        public CoffeeMachineService(
            IWeatherService weatherService,
            IDateTimeProvider dateTimeProvider,
            ICallCounter callCounter,
            ICoffeeMachineSettingsValidator settingsValidator,
            IOptions<CoffeeMachineSettings> settings,
            ILogger<ICoffeeMachineService> logger)
        {
            //constructor validation to prevent null injection
            _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _callCounter = callCounter ?? throw new ArgumentNullException(nameof(callCounter));
            _settingsValidator = settingsValidator ?? throw new ArgumentNullException(nameof(settingsValidator));
            _logger = logger;
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

            // Validate settings upon construction
            _settingsValidator.ValidateSettings(_settings);
        }

        public async Task<CoffeeMachineResponse> MakeCoffee()
        {
            try
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
                string message = await _weatherService.IsHotWeatherAsync() ? _settings.HotWeatherMessage : _settings.NormalWeatherMessage;

                return new CoffeeMachineResponse
                {
                    Message = message,
                    StatusCode = SpecalHttpCodes.OK,
                    Prepared = _dateTimeProvider.Now.ToCustomIsoFormat(_settings.DateTimeFormatDefault)
                };
            }
            catch (HttpRequestException httpEx)
            {
                // Handle specific HTTP request exceptions
                _logger.LogError($"HTTP request error in MakeCoffee: {httpEx.Message}");
                return new CoffeeMachineResponse
                {
                    Message = ErrorContactingWheatherServiceMessage,
                    StatusCode = SpecalHttpCodes.ServiceUnavailable
                };
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                _logger.LogError($"Unexpected error in MakeCoffee: {ex.Message}");
                return new CoffeeMachineResponse
                {
                    Message = ErrorUnexpectedMessage,
                    StatusCode = SpecalHttpCodes.InternalServerError
                };
            }
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
