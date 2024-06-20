namespace CoffeeMachine.API.Services.Weather
{
    public class OpenWeatherApiSettingsValidator : IOpenWeatherApiSettingsValidator
    {
        public void ValidateSettings(OpenWheatherApiSettings openWheatherApiSettings)
        {
            if (openWheatherApiSettings == null)
                throw new ArgumentNullException(nameof(openWheatherApiSettings));

            if (string.IsNullOrEmpty(openWheatherApiSettings.ApiKey))
                throw new ArgumentException($"{nameof(openWheatherApiSettings.ApiKey)} must not be empty");

            if (string.IsNullOrEmpty(openWheatherApiSettings.TargetCity))
                throw new ArgumentException($"{nameof(openWheatherApiSettings.TargetCity)} must not be empty");

            if (openWheatherApiSettings.HotTemperatureDefinition < 0 )
                throw new ArgumentException($"{nameof(openWheatherApiSettings.HotTemperatureDefinition)} must be greater than 0");

            if (string.IsNullOrEmpty(openWheatherApiSettings.OpenWheatherApiBaseUrl))
                throw new ArgumentException($"{nameof(openWheatherApiSettings.OpenWheatherApiBaseUrl)} must not be empty");

        }
    }
}
