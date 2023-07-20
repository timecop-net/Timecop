# Timecop - Easy Date and Time Testing in C\#

Timecop is a small library that helps you test DateTime in a static, thread-safe, ambient context.

Timecop targets .NET Standard 2.0, has no external dependencies, and can be used with .NET Framework 4.5+ and any version of .NET and .NET Core.

Timecop has been inspired by the [timecop](https://github.com/travisjeffery/timecop) Ruby gem.

## Installation

You can install [Timecop](https://www.nuget.org/packages/Timecop/) from NuGet using the .NET CLI:

```
dotnet add package Timecop
```

##  Basic usage

Timecop allows you to freeze and travel in time. Just use the `Clock` class instead of `DateTime`to get the current time via `Now` or `UtcNow` properties, and manipulate time with the `Timecop` class in your tests.

```csharp
string Greet()
{
    var timeOfDay = Clock.Now.Hour switch // Use Clock instead of DateTime
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

### Freezing and resuming time

You can freeze the time so that it stops running for your tests until you call `Resume` or dispose the `Timecop` instance.

You freeze time with either an instance `Freeze` or a static `Frozen` method, which both have the same set of overloads. Both methods have the same effect, however the static `Frozen` creates an already frozen `Timecop` instance.

```csharp
using var tc = Timecop.Frozen(1990, 12, 2, 14, 38, 51, DateTimeKind.Local);

Clock.Now; // 1990-12-02 14:38:51

Thread.Sleep(TimeSpan.FromSeconds(3));

Clock.Now; // 1990-12-02 14:38:51 - still the same value

tc.Resume();

Thread.Sleep(TimeSpan.FromSeconds(3));

Clock.Now; // 1990-12-02 14:38:54 - time has changed
```

`Freeze` and `Frozen` have multiple overloads:

```csharp
// freeze at the current instant:
var frozenAt = tc.Freeze();

// freeze at the specified DateTime:
frozenAt = tc.Freeze(new DateTime(1990, 12, 2, 14, 38, 51, DateTimeKind.Utc));

// freeze at the specified date and time:
frozenAt = tc.Freeze(1990, 12, 2, 14, 38, 51, DateTimeKind.Utc);

// freeze at the specified date:
frozenAt = tc.Freeze(1990, 12, 2, DateTimeKind.Utc);

// freeze at the specified date or time using a builder:
frozenAt = tc.Freeze(o => o.On(1990, 12, 2)
                .At(14, 13, 51)
                .LocalTime());
```

### Traveling in time

Use  the `TravelBy` method to travel forward and backward in time:

```csharp
using var tc = Timecop.Frozen(1990, 12, 2, 14, 38, 51, DateTimeKind.Local);

tc.TravelBy(TimeSpan.FromDays(1));

Clock.Now; // 1990-12-03 14:38:51 - one day in the future
```

## License

Timecop was created by [Dmytro Khmara](https://dmytrokhmara.com) and is licensed under the [MIT license](LICENSE.txt).