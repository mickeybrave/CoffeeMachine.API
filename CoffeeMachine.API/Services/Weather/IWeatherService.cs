namespace CoffeeMachine.API.Services.Weather
{
    public interface IWeatherService
    {
        Task<bool> IsHotWeatherAsync();
    }
}