using System;

namespace TCop;

internal class TimecopClock : IClock
{
    private readonly Func<DateTime> _getUtcNow;

    public TimecopClock(Func<DateTime> getUtcNow)
    {
        _getUtcNow = getUtcNow;
    }

    /// <summary>Returns either current or pre-configured local time. Time can be pre-configured by using <see cref="T:TCop.Timecop" />.</summary>
    public DateTime Now => _getUtcNow().ToLocalTime();

    /// <summary>Returns either current or pre-configured UTC time. Time can be pre-configured by using <see cref="T:TCop.Timecop" />.</summary>
    public DateTime UtcNow => _getUtcNow();
}