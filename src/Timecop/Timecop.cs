using System;
using TCop.Core;
using TCop.Core.Context;
using TCop.Time;
using TCop.Time.Builder;

namespace TCop;

public class Timecop : IDisposable
{
    private readonly TimecopContextStore _contextStore = new();

    public static DateTime UtcNow => TimecopContextStore.AsyncContextUtcNow.DateTimeUtc;

    private PointInTime ConvertAndFreeze(LocalOrUtcDateTime? utcDateTime)
    {
        return _contextStore.Mutate((ref TimecopContext context, PointInTime utcNow) => context.Freeze(utcDateTime?.PointInTime ?? utcNow));
    }

    private static DateTime ConvertToSpecificKind(DateTime dateTime, DateTimeKind kind)
    {
        if (kind == DateTimeKind.Utc)
            return dateTime.ToUniversalTime();

        return dateTime.ToLocalTime();
    }

    /// <summary>Moves in time backward or forward by the specified amount of time.</summary>
    /// <param name="duration">The amount of time to travel by. Can be positive or negative.</param>
    /// <returns>The UTC date and time the <see cref="T:TCop.Timecop" /> instance has traveled to.</returns>
    public DateTime TravelBy(TimeSpan duration)
    {
        return _contextStore.Mutate((ref TimecopContext context) => context.TravelBy(duration)).DateTimeUtc;
    }


    /// <summary>Freezes an instance of <see cref="T:TCop.Timecop" /> at the current time.</summary>
    /// <returns>The UTC date and time the <see cref="T:TCop.Timecop" /> instance was frozen at.</returns>
    public DateTime Freeze()
    {
        return ConvertAndFreeze(null).DateTimeUtc;
    }

    /// <summary>Freezes an instance of <see cref="T:TCop.Timecop" /> at the specified local or UTC time.</summary>
    /// <param name="freezeAt">The time to freeze at. Must represent either local or UTC time.</param>
    /// <returns>The date and time with the same kind as <paramref name="freezeAt" /> that the <see cref="T:TCop.Timecop" /> instance was frozen at.</returns>
    /// <exception cref="T:TCop.Core.Time.InvalidDateTimeKindException">
    ///     <paramref name="freezeAt" /> has the kind of Unspecified.
    /// </exception>
    public DateTime Freeze(DateTime freezeAt)
    {
        var frozenAt = ConvertAndFreeze(new LocalOrUtcDateTime(freezeAt));
        return ConvertToSpecificKind(frozenAt.DateTimeUtc, freezeAt.Kind);
    }

    /// <summary>Freezes an instance of <see cref="T:TCop.Timecop" /> at the specified local or UTC time.</summary>
    /// <param name="year">The year (1 through 9999).</param>
    /// <param name="month">The month (1 through 12).</param>
    /// <param name="day">The day (1 through the number of days in <paramref name="month" />).</param>
    /// <param name="hour">The hours (0 through 23).</param>
    /// <param name="minute">The minutes (0 through 59).</param>
    /// <param name="second">The seconds (0 through 59).</param>
    /// <param name="kind">One of the enumeration values that indicates whether <paramref name="year" />, <paramref name="month" />, <paramref name="day" />, <paramref name="hour" />, <paramref name="minute" /> and <paramref name="second" /> specify a local time, Coordinated Universal Time (UTC), or neither.</param>
    /// <returns>The date and time with the kind of <paramref name="kind" /> that the <see cref="T:TCop.Timecop" /> instance was frozen at.</returns>
    /// <exception cref="T:TCop.Core.Time.InvalidDateTimeKindException">
    ///     <paramref name="kind" /> has the kind of Unspecified.
    /// </exception>
    public DateTime Freeze(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
    {
        return Freeze(new DateTime(year, month, day, hour, minute, second, kind));
    }

    /// <summary>Freezes an instance of <see cref="T:TCop.Timecop" /> at the specified local or UTC time.</summary>
    /// <param name="year">The year (1 through 9999).</param>
    /// <param name="month">The month (1 through 12).</param>
    /// <param name="day">The day (1 through the number of days in <paramref name="month" />).</param>
    /// <param name="kind">One of the enumeration values that indicates whether <paramref name="year" />, <paramref name="month" />, <paramref name="day" />, <paramref name="hour" />, <paramref name="minute" /> and <paramref name="second" /> specify a local time, Coordinated Universal Time (UTC), or neither.</param>
    /// <returns>The date and time with the kind of <paramref name="kind" /> that the <see cref="T:TCop.Timecop" /> instance was frozen at.</returns>
    /// <exception cref="T:TCop.Core.Time.InvalidDateTimeKindException">
    ///     <paramref name="kind" /> has the kind of Unspecified.
    /// </exception>
    public DateTime Freeze(int year, int month, int day, DateTimeKind kind)
    {
        return Freeze(new DateTime(year, month, day, 0, 0, 0, kind));
    }

    /// <summary>Freezes an instance of <see cref="T:TCop.Timecop" /> at the specified local or UTC time.</summary>
    /// <param name="config">The function to configure the time to freeze at.</param>
    /// <returns>The date and time with the kind of specified in <paramref name="config" /> that the <see cref="T:TCop.Timecop" /> instance was frozen at.</returns>
    public DateTime Freeze(Action<PointInTimeBuilder> config)
    {
        var builder = new PointInTimeBuilder();
        config(builder);
        var frozenAt = Freeze(builder.Build(out var kind).DateTimeUtc);
        return ConvertToSpecificKind(frozenAt, kind);
    }

    /// <summary>Resumes the flow of time of a frozen instance of <see cref="T:TCop.Timecop" />.</summary>
    /// <returns>The UTC date and time the <see cref="T:TCop.Timecop" /> instance represented when it was resumed.</returns>
    public DateTime Resume()
    {
        return _contextStore.Mutate((ref TimecopContext context, PointInTime now) => context.Resume(now)).DateTimeUtc;
    }

    /// <summary>Creates an instance of <see cref="T:TCop.Timecop" /> and freezes it at the current time.</summary>
    /// <returns>A frozen <see cref="T:TCop.Timecop" /> instance.</returns>
    public static Timecop Frozen()
    {
        var timecop = new Timecop();
        timecop.Freeze();
        return timecop;
    }

    /// <summary>Creates an instance of <see cref="T:TCop.Timecop" /> and freezes it at the specified local or UTC time.</summary>
    /// <param name="frozenAt">The time to freeze at. Must represent either local or UTC time.</param>
    /// <returns>A frozen <see cref="T:TCop.Timecop" /> instance.</returns>
    /// <exception cref="T:TCop.DateTimeUtils.InvalidDateTimeKindException">
    ///     <paramref name="frozenAt" /> has the kind of Unspecified.
    /// </exception>
    public static Timecop Frozen(DateTime frozenAt)
    {
        var timecop = new Timecop();
        timecop.Freeze(frozenAt);
        return timecop;
    }

    /// <summary>Creates an instance of <see cref="T:TCop.Timecop" /> and freezes it at the specified local or UTC time.</summary>
    /// <param name="year">The year (1 through 9999).</param>
    /// <param name="month">The month (1 through 12).</param>
    /// <param name="day">The day (1 through the number of days in <paramref name="month" />).</param>
    /// <param name="hour">The hours (0 through 23).</param>
    /// <param name="minute">The minutes (0 through 59).</param>
    /// <param name="second">The seconds (0 through 59).</param>
    /// <param name="kind">One of the enumeration values that indicates whether <paramref name="year" />, <paramref name="month" />, <paramref name="day" />, <paramref name="hour" />, <paramref name="minute" /> and <paramref name="second" /> specify a local time, Coordinated Universal Time (UTC), or neither.</param>
    /// <returns>A frozen <see cref="T:TCop.Timecop" /> instance.</returns>
    /// <exception cref="T:TCop.DateTimeUtils.InvalidDateTimeKindException">
    ///     <paramref name="kind" /> has the kind of Unspecified.
    /// </exception>
    public static Timecop Frozen(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
    {
        var timecop = new Timecop();
        timecop.Freeze(new DateTime(year, month, day, hour, minute, second, kind));
        return timecop;
    }

    /// <summary>Creates an instance of <see cref="T:TCop.Timecop" /> and freezes it at the specified local or UTC time.</summary>
    /// <param name="year">The year (1 through 9999).</param>
    /// <param name="month">The month (1 through 12).</param>
    /// <param name="day">The day (1 through the number of days in <paramref name="month" />).</param>
    /// <param name="kind">One of the enumeration values that indicates whether <paramref name="year" />, <paramref name="month" />, <paramref name="day" />, <paramref name="hour" />, <paramref name="minute" /> and <paramref name="second" /> specify a local time, Coordinated Universal Time (UTC), or neither.</param>
    /// <returns>A frozen <see cref="T:TCop.Timecop" /> instance.</returns>
    /// <exception cref="T:TCop.DateTimeUtils.InvalidDateTimeKindException">
    ///     <paramref name="kind" /> has the kind of Unspecified.
    /// </exception>
    public static Timecop Frozen(int year, int month, int day, DateTimeKind kind)
    {
        var timecop = new Timecop();
        timecop.Freeze(new DateTime(year, month, day, 0, 0, 0, kind));
        return timecop;
    }

    /// <summary>Creates an instance of <see cref="T:TCop.Timecop" /> and freezes it at the specified local or UTC time.</summary>
    /// <param name="pointInTimeBuilder">The function to configure the time to freeze at.</param>
    /// <returns>A frozen <see cref="T:TCop.Timecop" /> instance.</returns>
    public static Timecop Frozen(Action<PointInTimeBuilder> pointInTimeBuilder)
    {
        var timecop = new Timecop();
        timecop.Freeze(pointInTimeBuilder);
        return timecop;
    }

    public void Dispose()
    {
        _contextStore.ResetContext();
    }
}