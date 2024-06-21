namespace CoffeeMachine.API.Services.CoffeMachine.Utilities
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Today => DateTime.Today;
        public DateTime Now => DateTime.Now;
    }
}
