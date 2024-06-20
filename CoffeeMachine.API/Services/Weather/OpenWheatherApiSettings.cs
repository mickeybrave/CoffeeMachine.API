namespace CoffeeMachine.API.Services.Weather
{
    public class OpenWheatherApiSettings
    {
        public string ApiKey { get; set; }
        public int HotTemperatureDefinition { get; set; }
        public string TargetCity { get; set; }
        public string OpenWheatherApiBaseUrl { get; set; }
        
    }
}