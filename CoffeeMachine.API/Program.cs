using CoffeeMachine.API.Infra;
using CoffeeMachine.API.Services.CoffeMachine;
using CoffeeMachine.API.Services.Weather;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
.AddControllers()
.AddJsonOptions(o =>
{
    //o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    //o.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    //o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    //o.JsonSerializerOptions.WriteIndented = true;
    //o.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});


builder.Services.AddSingleton<ICoffeeMachineService, CoffeeMachineService>();
builder.Services.AddSingleton<IWeatherService, WeatherService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<RemoveTraceIdMiddleware>();//we need that to remove traceId in the case of result 500 because it is not required by requirements

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }// we need this to aviod an issue in our integration tests because tests framework referencing to the wrong Program. The issue described here: https://stackoverflow.com/questions/69991983/deps-file-missing-for-dotnet-6-integration-tests
