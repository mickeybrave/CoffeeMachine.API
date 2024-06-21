# Important notes

## Design
The main idea was to use SOLID principles to make the code clean, testable, loose coupling, with single responsibilities, simple and easy to understand.

### Design patterns used

#### Dependency Injection

Most of the classes had constructor dependency injection for loose coupling and makes the code more testable.

Example: **WeatherService**

#### Interface Segregation Principle

This principle/pattern suggests that no client should be forced to depend on methods it does not use. Instead of one large interface, many small interfaces are preferred.

Example: **IWeatherHttpClient**

#### Adapter Pattern

This pattern allows the interface of an existing class to be used as another interface. It is often used to make existing classes work with others without modifying their source code.

Example: **WeatherHttpClient** 

#### Decorator Pattern

This pattern allows behavior to be added to an individual object, dynamically, without affecting the behavior of other objects from the same class.

Example: **RemoveTraceIdMiddleware**

Middleware like RemoveTraceIdMiddleware can be seen as a decorator in the request pipeline, adding additional behavior to HTTP requests and responses.

#### Strategy Pattern

This pattern enables selecting an algorithm's behavior at runtime. It defines a family of algorithms, encapsulates each one, and makes them interchangeable.

Example: **CoffeeMachineService**

Different services for weather data retrieval, validation, and handling different date conditions can be seen as strategies used by the CoffeeMachineService.

#### Facade Pattern
This pattern provides a simplified interface to a complex subsystem.
Example: **WeatherService**

The WeatherService can be seen as a facade that simplifies interactions with multiple components like IWeatherHttpClient, IOpenWeatherApiSettingsValidator, and logging.


## Code structure:

I've tried to keep it as simple as possible, but with strict separation of responsibilities in order to keep each class thin with single responsibility.
Description of main classes used:

### Required infrastructure
> CoffeeMachineController: for response mapping and providing response to web client.
### Core
> WeatherService: Provides weather-related operations, checking if the weather is hot based on city data.
> WeatherHttpClient: Handles HTTP requests to the OpenWeatherMap API for fetching weather data.
### Settings
> OpenWeatherApiSettingsValidator: Validates the settings for the OpenWeather API.
> OpenWheatherApiSettings: Holds configuration settings for the OpenWeather API.
### Utility & Other Services
> CoffeeMachineService: Manages the logic for making coffee based on weather conditions and call counts.
> DateTimeProvider: Provides the current date and time.
> CallCounter: Manages the count of calls to specific services.

 

## Security Key

I decided to keep it in configuration directly to reduce compelxity.
I real development we'd keep it in Azure Vault or Envronment Variable, but both options problematic when someone else will run it.

## Unit and Integration tests

I implemented both. Some may test the same behavior, but wanted to show that we can test different ways.

