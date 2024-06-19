using System.Text.Json.Nodes;
using System.Text.Json;

namespace CoffeeMachine.API.Infra
{
    public class RemoveTraceIdMiddleware
    {
        private readonly RequestDelegate _next;

        public RemoveTraceIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using (var newBodyStream = new MemoryStream())
            {
                context.Response.Body = newBodyStream;

                await _next(context);

                context.Response.Body = originalBodyStream;
                newBodyStream.Seek(0, SeekOrigin.Begin);

                var newBody = new StreamReader(newBodyStream).ReadToEnd();
                var jsonDocument = JsonDocument.Parse(newBody);
                var rootElement = jsonDocument.RootElement;

                using (var modifiedStream = new MemoryStream())
                {
                    using (var writer = new Utf8JsonWriter(modifiedStream))
                    {
                        writer.WriteStartObject();

                        foreach (var property in rootElement.EnumerateObject())
                        {
                            if (property.Name != "traceId")
                            {
                                property.WriteTo(writer);
                            }
                        }

                        writer.WriteEndObject();
                    }

                    modifiedStream.Seek(0, SeekOrigin.Begin);
                    var modifiedBody = new StreamReader(modifiedStream).ReadToEnd();
                    context.Response.ContentLength = modifiedBody.Length;
                    await context.Response.WriteAsync(modifiedBody);
                }
            }
        }
    }
}
