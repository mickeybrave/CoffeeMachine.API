using CoffeeMachine.API.Services.CoffeMachine.Utilities;

namespace CoffeeMachine.Tests
{
    public class DateTimeHelperTests
    {
        [Fact]
        public void ToCustomIsoFormat_WithDifferentTimeZone_ShouldFormatCorrectly()
        {
            // Arrange
            DateTime dateTime = new DateTime(2024, 06, 20, 22, 07, 04);

            // Act
            string formattedDateTime = dateTime.ToCustomIsoFormat("yyyy-MM-ddTHH:mm:sszzz");

            // Assert
            Assert.Equal("2024-06-20T22:07:04+1200", formattedDateTime);
        }
    }
}
