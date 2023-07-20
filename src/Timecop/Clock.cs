using System;

namespace TCop;

public class Clock
{
    /// <summary>Returns either current or pre-configured local time. Time can be pre-configured by using <see cref="T:TCop.Timecop" />.</summary>
    public static DateTime Now => Timecop.UtcNow.ToLocalTime();

    /// <summary>Returns either current or pre-configured UTC time. Time can be pre-configured by using <see cref="T:TCop.Timecop" />.</summary>
    public static DateTime UtcNow => Timecop.UtcNow;
}