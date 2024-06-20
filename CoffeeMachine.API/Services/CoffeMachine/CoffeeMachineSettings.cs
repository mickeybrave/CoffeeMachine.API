namespace CoffeeMachine.API.Services.CoffeMachine
{
    public class CoffeeMachineSettings
    {
        public int EverySpecialNumber { get; set; }
        public int SpecialDateMonth { get; set; }
        public int SpecialDateDay { get; set; }
        public string HotWeatherMessage { get; set; }
        public string NormalWeatherMessage { get; set; }
        public string DateTimeFormatDefault { get; set; }
    }
}
