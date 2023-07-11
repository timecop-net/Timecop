# Timecop - Easy Date and Time Testing in C\#


Timecop is a small library that helps you test DateTime in a static, thread-safe, ambient context.

Timecop targets .NET Standard 2.0, has no external dependencies, and can be used with .NET Framework 4.5+ and any version of .NET and .NET Core.

## Installation

You can install [Timecop](https://www.nuget.org/packages/Timecop/) from NuGet using the .NET CLI:

```
dotnet add package Timecop
```

##  Usage

Timecop allows freezing and travelling in time. It gives you two classes:
1. `Clock` - use it instead of `DateTime.Now` and `DateTime.UtcNow`to get the current time
2. `TimeCop`- use it to manipulate time in tests


Timecop can be used in a static context, the same way you would use DateTime:

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

using var tc = Timecop.Frozen(o => o.At(14,0,0).LocalTime());

Greet(); // Good afternoon!

tc.TravelBy(TimeSpan.FromHours(6)); // travel to 8pm local time

Greet(); // Good evening!
```





