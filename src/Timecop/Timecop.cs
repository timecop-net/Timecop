using System;

namespace TCop;

public class Timecop: IDisposable
{
    private readonly TimecopContextStore _contextStore = new();

    public static DateTime UtcNow => TimecopContextStore.AsyncContextUtcNow;

    public void TravelBy(TimeSpan duration)
    {
        _contextStore.Mutate((ref TimecopContext context) => context.TravelBy(duration));
    }

    private void ConvertAndFreeze(UtcDateTime? utcDateTime)
    {
        _contextStore.Mutate((ref TimecopContext context, DateTime utcNow) => context.Freeze(utcDateTime?.UtcValue ?? utcNow));
    }
    
    public Timecop Freeze(DateTime? freezeAt = null)
    {
        ConvertAndFreeze(freezeAt != null ? new UtcDateTime(freezeAt.Value) : null);
        return this;
    }

    public Timecop Freeze(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
    {
        return Freeze(new DateTime(year, month, day, hour, minute, second, kind));
    }

    public Timecop Freeze(int year, int month, int day, DateTimeKind kind)
    {
        return Freeze(new DateTime(year, month, day, 0, 0, 0, kind));
    }

    public void Unfreeze()
    {
        _contextStore.Mutate((ref TimecopContext context, DateTime utcNow) => context.Unfreeze(utcNow));
    }


    public static Timecop Frozen(DateTime? frozenAt = null)
    {
        return new Timecop().Freeze(frozenAt);
    }

    public static Timecop Frozen(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
    {
        return new Timecop().Freeze(new DateTime(year, month, day, hour, minute, second, kind));
    }

    public static Timecop Frozen(int year, int month, int day, DateTimeKind kind)
    {
        return new Timecop().Freeze(new DateTime(year, month, day, 0, 0, 0, kind));
    }

    public static Timecop Frozen(Action<DateTimeBuilder> config)
    {
        var builder = new DateTimeBuilder();
        config(builder);
        return new Timecop().Freeze(builder.Build());
    }

    public void Dispose()
    {
        _contextStore.ResetContext();
    }
}