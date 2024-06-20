namespace CoffeeMachine.API.Services.CoffeMachine.Utilities
{
    public interface ICallCounter
    {
        int Increment();
        void Reset();
    }
}
