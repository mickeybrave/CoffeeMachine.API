namespace CoffeeMachine.API.Services.CoffeMachine.Utilities
{
    public static class DateTimeHelper
    {
        public static string ToCustomIsoFormat(this DateTime dateTime, string formatt)
        {
            // Format the date-time string according to the provided format
            string formattedDatetime = dateTime.ToString(formatt);

            // Adjust the format of the time zone offset to ensure it's in hhmm format according to the requirement
            if (formattedDatetime.Length >= 5 && (formattedDatetime.Substring(formattedDatetime.Length - 6, 6).StartsWith("+") || formattedDatetime.Substring(formattedDatetime.Length - 6, 6).StartsWith("-")))
            {
                formattedDatetime = formattedDatetime.Substring(0, formattedDatetime.Length - 3) + formattedDatetime.Substring(formattedDatetime.Length - 2);
            }

            return formattedDatetime;
        }
    }
}
