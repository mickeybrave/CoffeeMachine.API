using Microsoft.Extensions.Options;

namespace CoffeeMachine.API.Services.Weather
{
    public class WeatherHttpClient : IWeatherHttpClient, IDisposable
    {
        private readonly HttpClient _httpClient;

        public WeatherHttpClient(HttpClient httpClient, IOptions<OpenWheatherApiSettings> settings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            var config = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _httpClient.BaseAddress = new Uri(config.OpenWheatherApiBaseUrl);
        }

        public async Task<HttpResponseMessage> GetWeatherDataAsync(string city, string apiKey)
        {
            return await _httpClient.GetAsync($"weather?q={city}&units=metric&appid={apiKey}");
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }

}
