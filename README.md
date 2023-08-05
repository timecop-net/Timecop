# Timecop - Easy Date and Time Testing in C\#

Timecop is a small library that helps to test the code that depends on DateTime. Timecop allows to freeze time and travel in time.

Timecop targets .NET Standard 2.0, has no external dependencies, and can be used with .NET Framework 4.5+ and any version of .NET and .NET Core.

Timecop has been inspired by the [timecop](https://github.com/travisjeffery/timecop) Ruby gem.

## Installation

You can install [Timecop](https://www.nuget.org/packages/Timecop/) from NuGet using your IDE or the .NET CLI:

```
dotnet add package Timecop
```

## Basics and usage

To test with Timecop, your code must get the current time using either:
- The `IClock` interface
- The static `Clock` class
 
### Usage with the `IClock` interface

Timecop provides the `IClock` interface that exposes the `Now` and `UtcNow` properties.

Here's how to use it:
1. Pass the instance of `IClock` to your code as a method or a constructur parameter and use it to get the current time
1. In your tests, configure the `Timecop` instance and pass its `Clock` property into the code under test
1. When running in production, pass the `IClock` implementaton that always returns the current time. You can use the [Timecop.Extensions.DependencyInjection](https://github.com/timecop-net/Timecop.Extensions.DependencyInjection) package to register such implementation with the DI container in one line of code

Here's an example:

```csharp
string Greet(IClock clock)
{
    var timeOfDay = clock.Now.Hour switch
    {
        >= 0 and < 6 => "night",
        >= 6 and < 12 => "morning",
        >= 12 and < 18 => "afternoon",
        _ => "evening"
    };

    return $"Good {timeOfDay}!";
}

// freeze at 2pm local time:
using var tc = Timecop.Frozen(o => o.At(14,0,0).LocalTime()); 

Greet(tc.Clock); // Good afternoon!

// travel to 8pm local time:
tc.TravelBy(TimeSpan.FromHours(6)); 

Greet(tc.Clock); // Good evening!
```

### Usage with the `Clock` class

Timecop provides the static `Clock` class that you can use instead of `DateTime` to get the current local or UTC time. Despite `Clock` being a static class, it is safe to use in tests that run in parallel as it uses [AsyncLocal](https://learn.microsoft.com/en-us/dotnet/api/system.threading.asynclocal-1) under the hood.

Here's how to use it:
1. Replace the calls to `DateTime.Now` and `DateTime.UtcNow` with the calls to `Clock.Now` and `Clock.UtcNow`
1. In your tests, configure the `Timecop` instance

Example:

```csharp
string Greet()
{
    var timeOfDay = Clock.Now.Hour switch // Use Clock.Now instead of DateTime.Now
    {
        >= 0 and < 6 => "night",
        >= 6 and < 12 => "morning",
        >= 12 and < 18 => "afternoon",
        _ => "evening"
    };

    return $"Good {timeOfDay}!";
}

// freeze at 2pm local time:
using var tc = Timecop.Frozen(o => o.At(14,0,0).LocalTime()); 

Greet(); // Good afternoon!

// travel to 8pm local time:
tc.TravelBy(TimeSpan.FromHours(6)); 

Greet(); // Good evening!
```

## Available methods

Timecop allows to manipulate time in any imaginable way. Use it to freeze time, travel in time, and resume the flow of time.

### Freezing and resuming time

You can freeze the time so that it stops running for your tests until you call `Resume`, `Reset`, or dispose the `Timecop` instance.

To freeze the time, use either an instance `Freeze` or a static `Frozen` method, both having the same set of overloads. Both methods have the same effect, however the static `Frozen` creates an already frozen `Timecop` instance.

```csharp
using var tc = Timecop.Frozen(1990, 12, 2, 14, 38, 51, DateTimeKind.Local);

Clock.Now; // 1990-12-02 14:38:51

Thread.Sleep(TimeSpan.FromSeconds(3));

Clock.Now; // 1990-12-02 14:38:51 - still the same value

tc.Resume(); // Resumes the flow of time.

Thread.Sleep(TimeSpan.FromSeconds(3));

Clock.Now; // 1990-12-02 14:38:54 - moved ~3 seconds forward
```

`Freeze` and `Frozen` have multiple overloads:

```csharp
// freeze at the current instant:
var frozenAt = tc.Freeze();

// freeze at the specific DateTime:
frozenAt = tc.Freeze(new DateTime(1990, 12, 2, 14, 38, 51, DateTimeKind.Utc));

// freeze at the specific date and time:
frozenAt = tc.Freeze(1990, 12, 2, 14, 38, 51, DateTimeKind.Utc);

// freeze at the specific date:
frozenAt = tc.Freeze(1990, 12, 2, DateTimeKind.Utc);

// freeze at the specific point in time configured with a PointInTimeBuilder:
frozenAt = tc.Freeze(o => o.On(1990, 12, 2)
                .At(14, 13, 51)
                .InLocalZone());
```

### Traveling in time

Use the `TravelTo` method to travel to the specific point in time:

```csharp
// travel to the specific DateTime:
traveledTo = tc.TravelTo(new DateTime(1990, 12, 2, 14, 38, 51, DateTimeKind.Utc));

// travel to the specific date and time:
traveledTo = tc.TravelTo(1990, 12, 2, 14, 38, 51, DateTimeKind.Utc);

// travel to the specific date:
traveledTo = tc.TravelTo(1990, 12, 2, DateTimeKind.Utc);

// travel to the specific point in time configured with a PointInTimeBuilder:
traveledTo = tc.TravelTo(o => o.On(1990, 12, 2)
                .At(14, 13, 51)
                .InLocalZone());
```

Use the `TravelBy` method to travel forward and backward in time:

```csharp
using var tc = Timecop.Frozen(1990, 12, 2, 14, 38, 51, DateTimeKind.Local);

tc.TravelBy(TimeSpan.FromDays(1));

Clock.Now; // 1990-12-03 14:38:51 - one day in the future
```

Several shorthand `Travel` methods exist:

```csharp
tc.TravelDays(5);

tc.TravelHours(20);

tc.TravelMinutes(31);

tc.TravelSeconds(45);

tc.TravelMilliseconds(350);
```

### Using PointInTimeBuilder

`Freeze`, `Frozen`, and `TravelTo` methods each accept a lambda that allows to configure the time with the `PointInTimeBuilder` class.

Use `PointInTimeBuilder` to construct a DateTime from its components. When using `At` and `On` methods, always specify whether the time is local or UTC using `InLocalZone` or `InUtc` methods.

```csharp
// When only the date matters:
builder.On(1990, 12, 2).InLocalZone(); // will use the specified date and current time

// When only the time matters:
builder.At(14, 13, 51).InLocalZone(); // will use the specified time and current date

// When neither the date nor time matters, but the DateTime must be in the future or in the past:
builder.InTheFuture();

builder.InThePast();
```

## Usage with NodaTime

Check out [Timecop.NodaTime](https://github.com/timecop-net/Timecop.NodaTime) which is a version of Timecop with the NodaTime API.

## License

Timecop was created by [Dmytro Khmara](https://dmytrokhmara.com) and is licensed under the [MIT license](LICENSE.txt).