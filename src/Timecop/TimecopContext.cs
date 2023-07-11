using System;

namespace TCop;

public struct TimecopContext
{
    private DateTime? _lastRealTimeFrozenAt;

    private TimeSpan _timeTravelDelta;

    private TimeSpan _totalRealTimePassedWhileFrozen;
    
    public TimecopContext()
    {
        _timeTravelDelta = TimeSpan.Zero;
        _totalRealTimePassedWhileFrozen = TimeSpan.Zero;
        _lastRealTimeFrozenAt = null;
    }

    public void TravelBy(TimeSpan duration)
    {
        _timeTravelDelta = _timeTravelDelta.Add(duration);
    }

    public DateTime GetUtcNow(DateTime realUtcNow)
    {
        var baseTime = _lastRealTimeFrozenAt ?? realUtcNow;

        var baseTimeWithDelta = baseTime.Add(_timeTravelDelta);

        return baseTimeWithDelta.Subtract(_totalRealTimePassedWhileFrozen);
    }

    public DateTime Freeze(DateTime utcNow)
    {
        _lastRealTimeFrozenAt = utcNow;

        return GetUtcNow(utcNow);
    }

    public void Unfreeze(DateTime utcNow)
    {
        if (!_lastRealTimeFrozenAt.HasValue)
            return;

        var realTimePassedInFrozenState = utcNow.Subtract(_lastRealTimeFrozenAt.Value);

        _totalRealTimePassedWhileFrozen = _totalRealTimePassedWhileFrozen.Add(realTimePassedInFrozenState);

        _lastRealTimeFrozenAt = null;
    }
}