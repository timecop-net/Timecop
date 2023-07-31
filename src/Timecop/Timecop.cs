using System;
using TCop.Core;
using TCop.Core.Context;
using TCop.Time;
using TCop.Time.Builder;

namespace TCop;

public class Timecop : IDisposable
{
    private readonly TimecopContextStore _contextStore = new();

    public static DateTime UtcNow => TimecopContextStore.AsyncContextNow.DateTimeUtc;

    public Timecop()
    {
        Clock = new TimecopClock(() => _contextStore.InstanceContextNow.DateTimeUtc);
    }

    private static DateTime ConvertToSpecificKind(DateTime dateTime, DateTimeKind kind)
    {
        if (kind == DateTimeKind.Utc)
            return dateTime.ToUniversalTime();

        return dateTime.ToLocalTime();
    }

    /// <summary>
    /// Returns an <see cref="T:TCop.IClock" /> implementation that you can pass to your code in the tests.
    /// </summary>
    public IClock Clock { get; }

    /// <summary>Moves in time backward or forward by the specified amount of time.</summary>
    /// <param name="duration">The amount of time to travel by. Can be positive or negative.</param>
    /// <returns>The UTC date and time the time has traveled to.</returns>
    public DateTime TravelBy(TimeSpan duration)
    {
        return _contextStore.Mutate((ref TimecopContext context, PointInTime realNow) => context.TravelBy(duration, realNow)).DateTimeUtc;
    }


    /// <summary>Freezes the time at the current moment.</summary>
    /// <returns>The UTC date and time the time was frozen at.</returns>
    public DateTime Freeze()
    {
        return _contextStore.Mutate((ref TimecopContext context, PointInTime realNow) => context.Freeze(realNow)).DateTimeUtc;
    }

    /// <summary>Freezes the time at the specified local or UTC date and time.</summary>
    /// <param name="destination">The date and time to freeze at. Must represent either local or UTC time.</param>
    /// <returns>The date and time with the same kind as <paramref name="destination" /> that the <see cref="T:TCop.Timecop" /> instance was frozen at.</returns>
    /// <exception cref="T:TCop.Time.InvalidDateTimeKindException">
    ///     <paramref name="destination" /> has the kind of Unspecified.
    /// </exception>
    public DateTime Freeze(DateTime destination)
    {
        var frozenAt = _contextStore.Mutate((ref TimecopContext context, PointInTime realNow) => context.FreezeAt(new LocalOrUtcDateTime(destination).PointInTime, realNow));
        return ConvertToSpecificKind(frozenAt.DateTimeUtc, destination.Kind);
    }

    /// <summary>Freezes the time at the specified local or UTC time.</summary>
    /// <param name="year">The year (1 through 9999).</param>
    /// <param name="month">The month (1 through 12).</param>
    /// <param name="day">The day (1 through the number of days in <paramref name="month" />).</param>
    /// <param name="hour">The hours (0 through 23).</param>
    /// <param name="minute">The minutes (0 through 59).</param>
    /// <param name="second">The seconds (0 through 59).</param>
    /// <param name="kind">One of the enumeration values that indicates whether <paramref name="year" />, <paramref name="month" />, <paramref name="day" />, <paramref name="hour" />, <paramref name="minute" /> and <paramref name="second" /> specify a local time, Coordinated Universal Time (UTC), or neither.</param>
    /// <returns>The date and time with the kind of <paramref name="kind" /> that the time was frozen at.</returns>
    /// <exception cref="T:TCop.Time.InvalidDateTimeKindException">
    ///     <paramref name="kind" /> has the kind of Unspecified.
    /// </exception>
    public DateTime Freeze(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
    {
        return Freeze(new DateTime(year, month, day, hour, minute, second, kind));
    }

    /// <summary>Freezes the time at midnight at the provided date.</summary>
    /// <param name="year">The year (1 through 9999).</param>
    /// <param name="month">The month (1 through 12).</param>
    /// <param name="day">The day (1 through the number of days in <paramref name="month" />).</param>
    /// <param name="kind">One of the enumeration values that indicates whether <paramref name="year" />, <paramref name="month" />, <paramref name="day" />, <paramref name="hour" />, <paramref name="minute" /> and <paramref name="second" /> specify a local time, Coordinated Universal Time (UTC), or neither.</param>
    /// <returns>The date and time with the kind of <paramref name="kind" /> that the <see cref="T:TCop.Timecop" /> instance was frozen at.</returns>
    /// <exception cref="T:TCop.Time.InvalidDateTimeKindException">
    ///     <paramref name="kind" /> has the kind of Unspecified.
    /// </exception>
    public DateTime Freeze(int year, int month, int day, DateTimeKind kind)
    {
        return Freeze(new DateTime(year, month, day, 0, 0, 0, kind));
    }

    /// <summary>Freezes the time at the specified local or UTC date and time.</summary>
    /// <param name="configureDestination">The function to configure the date and time to freeze at.</param>
    /// <returns>The date and time with the kind of specified in <paramref name="configureDestination" /> that the <see cref="T:TCop.Timecop" /> instance was frozen at.</returns>
    public DateTime Freeze(Action<PointInTimeBuilder> configureDestination)
    {
        var builder = new PointInTimeBuilder();
        configureDestination(builder);
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
    /// <exception cref="T:TCop.Time.InvalidDateTimeKindException">
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
    /// <exception cref="T:TCop.Time.InvalidDateTimeKindException">
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
    /// <exception cref="T:TCop.Time.InvalidDateTimeKindException">
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

    /// <summary>Moves the time to the given local or UTC date and time.</summary>
    /// <param name="destination">The instant to travel to. Must represent either local or UTC time.</param>
    /// <returns>The date and time with the same kind as <paramref name="destination" /> that the time has traveled to.</returns>
    /// <exception cref="T:TCop.Time.InvalidDateTimeKindException">
    ///     <paramref name="destination" /> has the kind of Unspecified.
    /// </exception>
    public DateTime TravelTo(DateTime destination)
    {
        var frozenAt = _contextStore.Mutate((ref TimecopContext context, PointInTime realNow) => context.TravelTo(new LocalOrUtcDateTime(destination).PointInTime, realNow));
        return ConvertToSpecificKind(frozenAt.DateTimeUtc, destination.Kind);
    }

    /// <summary>Moves the time to the given local or UTC date and time.</summary>
    /// <param name="year">The year (1 through 9999).</param>
    /// <param name="month">The month (1 through 12).</param>
    /// <param name="day">The day (1 through the number of days in <paramref name="month" />).</param>
    /// <param name="hour">The hours (0 through 23).</param>
    /// <param name="minute">The minutes (0 through 59).</param>
    /// <param name="second">The seconds (0 through 59).</param>
    /// <param name="kind">One of the enumeration values that indicates whether <paramref name="year" />, <paramref name="month" />, <paramref name="day" />, <paramref name="hour" />, <paramref name="minute" /> and <paramref name="second" /> specify a local time, Coordinated Universal Time (UTC), or neither.</param>
    /// <returns>The date and time with the kind of <paramref name="kind" /> that the time has traveled to.</returns>
    /// <exception cref="T:TCop.Time.InvalidDateTimeKindException">
    ///     <paramref name="kind" /> has the kind of Unspecified.
    /// </exception>
    public DateTime TravelTo(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
    {
        return TravelTo(new DateTime(year, month, day, hour, minute, second, kind));
    }

    /// <summary>Moves the time to the midnight at the given date.</summary>
    /// <param name="year">The year (1 through 9999).</param>
    /// <param name="month">The month (1 through 12).</param>
    /// <param name="day">The day (1 through the number of days in <paramref name="month" />).</param>
    /// <param name="kind">One of the enumeration values that indicates whether <paramref name="year" />, <paramref name="month" />, <paramref name="day" />, <paramref name="hour" />, <paramref name="minute" /> and <paramref name="second" /> specify a local time, Coordinated Universal Time (UTC), or neither.</param>
    /// <returns>The date and time with the kind of <paramref name="kind" /> that the time has traveled to.</returns>
    /// <exception cref="T:TCop.Time.InvalidDateTimeKindException">
    ///     <paramref name="kind" /> has the kind of Unspecified.
    /// </exception>
    public DateTime TravelTo(int year, int month, int day, DateTimeKind kind)
    {
        return TravelTo(new DateTime(year, month, day, 0, 0, 0, kind));
    }

    /// <summary>Moves the time to the specified local or UTC date and time.</summary>
    /// <param name="configureDestination">The function to configure the date and time to travel to.</param>
    /// <returns>The date and time with the kind of specified in <paramref name="configureDestination" /> that the time has traveled to.</returns>
    public DateTime TravelTo(Action<PointInTimeBuilder> configureDestination)
    {
        var builder = new PointInTimeBuilder();
        configureDestination(builder);
        var frozenAt = TravelTo(builder.Build(out var kind).DateTimeUtc);
        return ConvertToSpecificKind(frozenAt, kind);
    }
}