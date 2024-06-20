namespace CoffeeMachine.API.Results
{
    //new enum created because where is no such enum that can represent code 418 code
    public enum SpecalHttpCodes
    {
        /// <summary>
        /// Response successfull
        /// </summary>
        OK = 200,
        /// <summary>
        ///  I'm a teapot response
        /// </summary>
        ImATeapot = 418,
        /// <summary>
        /// Service unavailable. the coffee machine is out of coffee;
        /// </summary>
        ServiceUnavailable = 503,
        /// <summary>
        /// Error happened in calling of internal wheather service
        /// </summary>
        InternalServerError = 500

    }
    public struct CoffeeMachineResponse
    {
        public SpecalHttpCodes StatusCode { get; set; }
        public string Message { get; set; }

        public string Prepared { get; set; }

        public CoffeeMachineResponse()
        {
            Message = "";
            Prepared = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz");
            StatusCode = SpecalHttpCodes.InternalServerError;
        }
    }

}
