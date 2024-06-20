using CoffeeMachine.API.Services.CoffeMachine;

namespace CoffeeMachine.API.Services.Weather
{
    public interface IOpenWeatherApiSettingsValidator
    {
        public void ValidateSettings(OpenWheatherApiSettings settings);
    }
}