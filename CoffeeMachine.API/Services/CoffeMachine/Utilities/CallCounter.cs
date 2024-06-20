namespace CoffeeMachine.API.Services.CoffeMachine.Utilities
{
    /// <summary>
    /// Made for logic isolation and prevention thread safity issues happened in unit tests
    /// </summary>
    public class CallCounter : ICallCounter
    {
        private int _count;

        public int Increment()
        {
            return Interlocked.Increment(ref _count);
        }

        public void Reset()
        {
            Interlocked.Exchange(ref _count, 0);
        }
    }
}
