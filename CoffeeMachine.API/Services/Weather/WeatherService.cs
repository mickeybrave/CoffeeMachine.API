using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace CoffeeMachine.API.Services.Weather
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherHttpClient _weatherHttpClient;
        private readonly IOpenWeatherApiSettingsValidator _openWeatherApiSettingsValidator;
        private readonly ILogger<WeatherService> _logger;
        private readonly OpenWheatherApiSettings _settings;
        private readonly string _apiKey;

        public WeatherService(IWeatherHttpClient weatherHttpClient,
            IOptions<OpenWheatherApiSettings> settings,
            IOpenWeatherApiSettingsValidator openWeatherApiSettingsValidator,
            ILogger<WeatherService> logger)
        {
            _weatherHttpClient = weatherHttpClient ?? throw new ArgumentNullException(nameof(weatherHttpClient));
            _openWeatherApiSettingsValidator = openWeatherApiSettingsValidator ?? throw new ArgumentNullException(nameof(openWeatherApiSettingsValidator));
            _logger = logger;
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

            // Validate settings using the validator
            _openWeatherApiSettingsValidator.ValidateSettings(_settings);

            // Set API key
            _apiKey = _settings.ApiKey;
        }

        public async Task<bool> IsHotWeatherAsync()
        {
            return await IsHotWeatherByCityAsync(_settings.TargetCity);
        }

        private async Task<bool> IsHotWeatherByCityAsync(string city)
        {
            try
            {
                var response = await _weatherHttpClient.GetWeatherDataAsync(city, _apiKey);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Received unsuccessful response from OpenWeatherMap API: {response.StatusCode}");
                    return false;
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var weatherData = JObject.Parse(jsonString);
                var temperature = weatherData["main"]["temp"].Value<double>();

                return temperature > _settings.HotTemperatureDefinition;
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exception
                _logger.LogError($"Error calling OpenWeatherMap API: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                _logger.LogError($"Error in IsHotWeatherAsync: {ex.Message}");
                throw;
            }
        }
    }

}
