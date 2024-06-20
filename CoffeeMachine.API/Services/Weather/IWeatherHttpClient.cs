namespace CoffeeMachine.API.Services.Weather
{
    public interface IWeatherHttpClient
    {
        Task<HttpResponseMessage> GetWeatherDataAsync(string city, string apiKey);
    }

}
