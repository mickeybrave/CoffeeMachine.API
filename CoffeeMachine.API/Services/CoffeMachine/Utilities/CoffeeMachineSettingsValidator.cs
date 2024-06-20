namespace CoffeeMachine.API.Services.CoffeMachine.Utilities
{
    public class CoffeeMachineSettingsValidator : ICoffeeMachineSettingsValidator
    {
        public void ValidateSettings(CoffeeMachineSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.EverySpecialNumber <= 0)
                throw new ArgumentException($"{nameof(settings.EverySpecialNumber)} must not be null or 0");

            if (settings.SpecialDateMonth < 1 || settings.SpecialDateMonth > 12)
                throw new ArgumentException($"{nameof(settings.SpecialDateMonth)} must be between 1 and 12");

            int daysInMonth;
            switch (settings.SpecialDateMonth)
            {
                case 1: // January
                case 3: // March
                case 5: // May
                case 7: // July
                case 8: // August
                case 10: // October
                case 12: // December
                    daysInMonth = 31;
                    break;
                case 4: // April
                case 6: // June
                case 9: // September
                case 11: // November
                    daysInMonth = 30;
                    break;
                case 2: // February
                        // Determine days in February considering leap year
                    daysInMonth = DateTime.IsLeapYear(DateTime.UtcNow.Year) ? 29 : 28;
                    break;
                default:
                    throw new ArgumentException("Invalid month value.");
            }

            if (settings.SpecialDateDay < 1 || settings.SpecialDateDay > daysInMonth)
                throw new ArgumentException($"{settings.SpecialDateDay} must be between 1 and {daysInMonth} for month {settings.SpecialDateMonth}");
        }
    }
}
