using CoffeeMachine.API.Infra;
using CoffeeMachine.API.Services.CoffeMachine;
using CoffeeMachine.API.Services.CoffeMachine.Utilities;
using CoffeeMachine.API.Services.Weather;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CoffeeMachineSettings>(builder.Configuration.GetSection("CoffeeMachineSettings"));// to be able to use it in CoffeeMachineService
builder.Services.Configure<OpenWheatherApiSettings>(builder.Configuration.GetSection("OpenWheatherApiSettings"));// to be able to use it in CoffeeMachineService

// Configure logging
builder.Logging.AddConsole(); // Example: Add console logging
builder.Logging.AddDebug();   // Example: Add debug logging

// Add services to the container.
builder.Services.AddControllers();


// Register HttpClient
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ICoffeeMachineService, CoffeeMachineService>();
builder.Services.AddSingleton<IWeatherService, WeatherService>();
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddSingleton<ICallCounter, CallCounter>();
builder.Services.AddSingleton<IOpenWeatherApiSettingsValidator, OpenWeatherApiSettingsValidator>();
builder.Services.AddSingleton<ICoffeeMachineSettingsValidator, CoffeeMachineSettingsValidator>();
builder.Services.AddHttpClient<IWeatherHttpClient, WeatherHttpClient>();




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
