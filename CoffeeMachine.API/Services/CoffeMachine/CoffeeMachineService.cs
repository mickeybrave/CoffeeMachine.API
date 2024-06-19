using CoffeeMachine.API.Results;
using CoffeeMachine.API.Services.Weather;

namespace CoffeeMachine.API.Services.CoffeMachine
{
    public class CoffeeMachineService : ICoffeeMachineService
    {
        private static int _callCount = 0;
        private readonly IWeatherService _weatherService;

        public CoffeeMachineService(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public CoffeMachineRespond MakeCoffee()
        {
            Interlocked.Increment(ref _callCount);// used insread of ++ to take care of multi thread access to coffee machine and more than 1 user or access from different client

            // Check for April 1st
            if (DateTime.Today.Month == 4 && DateTime.Today.Day == 1)
            {
                return new CoffeMachineRespond { StatusCode = SpecalHttpCodes.ImATeapot };
            }

            // Check if every fifth call
            if (_callCount % 5 == 0)
            {
                _callCount = 0;// prevent an issue when int reaches max value and as a result it will be min negative value -2,147,483,648
                return new CoffeMachineRespond { StatusCode = SpecalHttpCodes.ServiceUnavailable };
            }

            // Determine the message based on weather condition (Extra Credit)
            string message = "Your piping hot coffee is ready";
            if (_weatherService.IsHotWeather())
            {
                message = "Your refreshing iced coffee is ready";
            }

            // Prepare response
            return new CoffeMachineRespond
            {
                Message = message,
                StatusCode = SpecalHttpCodes.OK
            };

        }

      
    }
}
