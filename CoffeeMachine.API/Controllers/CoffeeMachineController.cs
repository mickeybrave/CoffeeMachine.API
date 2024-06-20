using CoffeeMachine.API.Results;
using CoffeeMachine.API.Services.CoffeMachine;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeMachine.API.Controllers
{
    [ApiController]
    [Route("")]
    public class CoffeeMachineController : ControllerBase
    {
        private readonly ICoffeeMachineService _coffeeMachineService;

        public CoffeeMachineController(ICoffeeMachineService coffeeMachineService)
        {
            _coffeeMachineService = coffeeMachineService;
        }

        [HttpGet("/brew-coffee")]
        public async Task<IActionResult> BrewCoffee()
        {
            var result = await _coffeeMachineService.MakeCoffee();

            switch (result.StatusCode)
            {
                case SpecalHttpCodes.ImATeapot:
                case SpecalHttpCodes.ServiceUnavailable:
                    return StatusCode((int)result.StatusCode);
                case SpecalHttpCodes.OK:
                    return Ok(new APIResult { Message = result.Message, Prepared = result.Prepared });
                default:
                    return StatusCode((int)SpecalHttpCodes.InternalServerError);
            }
        }
    }
}
