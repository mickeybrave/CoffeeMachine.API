using CoffeeMachine.API.Controllers;
using CoffeeMachine.API.Results;
using CoffeeMachine.API.Services.CoffeMachine;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CoffeeMachine.Tests
{
    public class CoffeeMachineControllerTests
    {
        [Fact]
        public async Task MakeCoffee_ReturnsOkWithPipingHotCoffee()
        {
            // Arrange
            var mockService = new Mock<ICoffeeMachineService>();
            mockService.Setup(service => service.MakeCoffee())
                       .ReturnsAsync(new CoffeeMachineResponse { StatusCode = SpecalHttpCodes.OK, Message = "Your piping hot coffee is ready", Prepared = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") });

            var controller = new CoffeeMachineController(mockService.Object);

            // Act
            var result = await controller.BrewCoffee() as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            var responseObject = result.Value.Should().BeAssignableTo<object>().Subject;
            responseObject.Should().NotBeNull();

            ((APIResult)responseObject).Message.Should().Be("Your piping hot coffee is ready");
            ((APIResult)responseObject).Prepared.Should().NotBeNull();
        }

        [Fact]
        public async Task MakeCoffee_ReturnsImATeapotOnAprilFirst()
        {
            // Arrange
            var mockService = new Mock<ICoffeeMachineService>();
            mockService.Setup(service => service.MakeCoffee())
                       .ReturnsAsync(new CoffeeMachineResponse { StatusCode = SpecalHttpCodes.ImATeapot });

            var controller = new CoffeeMachineController(mockService.Object);

            // Act
            var result = await controller.BrewCoffee() as StatusCodeResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(418);
        }

        [Fact]
        public async Task MakeCoffee_ReturnsServiceUnavailableOnFifthCall()
        {
            // Arrange
            var mockService = new Mock<ICoffeeMachineService>();
            mockService.SetupSequence(service => service.MakeCoffee())
                       .ReturnsAsync(new CoffeeMachineResponse { StatusCode = SpecalHttpCodes.OK })
                       .ReturnsAsync(new CoffeeMachineResponse { StatusCode = SpecalHttpCodes.OK })
                       .ReturnsAsync(new CoffeeMachineResponse { StatusCode = SpecalHttpCodes.OK })
                       .ReturnsAsync(new CoffeeMachineResponse { StatusCode = SpecalHttpCodes.OK })
                       .ReturnsAsync(new CoffeeMachineResponse { StatusCode = SpecalHttpCodes.ServiceUnavailable });

            var controller = new CoffeeMachineController(mockService.Object);

            // Act
            for (int i = 0; i < 4; i++)
            {
                controller.BrewCoffee();
            }
            var result = await controller.BrewCoffee() as StatusCodeResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(503);
        }

        [Fact]
        public async Task MakeCoffee_ReturnsInternalServerErrorForUnknownStatusCode()
        {
            // Arrange
            var mockService = new Mock<ICoffeeMachineService>();
            mockService.Setup(service => service.MakeCoffee())
                       .ReturnsAsync(new CoffeeMachineResponse { StatusCode = (SpecalHttpCodes)999 });

            var controller = new CoffeeMachineController(mockService.Object);

            // Act
            var result = await controller.BrewCoffee() as StatusCodeResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(500); // InternalServerError
        }
    }
}