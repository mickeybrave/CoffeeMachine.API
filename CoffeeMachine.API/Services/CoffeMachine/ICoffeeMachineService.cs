using CoffeeMachine.API.Results;

namespace CoffeeMachine.API.Services.CoffeMachine
{
    public interface ICoffeeMachineService
    {
        CoffeMachineRespond MakeCoffee();
    }
}