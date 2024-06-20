namespace CoffeeMachine.API.Services.CoffeMachine.Utilities
{
    public interface IDateTimeProvider
    {
        DateTime Today { get; }
        DateTime UtcNow { get; }
    }
}
