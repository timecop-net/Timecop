using System;

namespace TCop;

public class Clock
{
    public static DateTime Now => Timecop.UtcNow.ToLocalTime();
    public static DateTime UtcNow => Timecop.UtcNow;
}