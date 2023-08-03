using FluentAssertions;
using TCop.Time.Builder;

namespace TCop.Tests.Time;

public class PointInTimeBuilderTests
{
    private static readonly TimeSpan DateTimeComparisonPrecision = TimeSpan.FromMilliseconds(50);

    private readonly PointInTimeBuilder _builder = new();

    [Fact]
    public void LocalTime_ShouldReturnCurrentLocalTime()
    {
        _builder.InLocalZone();

        _builder.Build(out var kind).DateTimeUtc.ToLocalTime().Should().BeCloseTo(DateTime.Now, DateTimeComparisonPrecision);
        kind.Should().Be(DateTimeKind.Local);
    }

    [Fact]
    public void UtcTime_ShouldReturnCurrentUtcTime()
    {
        _builder.InUtc();

        _builder.Build(out var kind).DateTimeUtc.Should().BeCloseTo(DateTime.UtcNow, DateTimeComparisonPrecision);
        kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Build_OnWasCalled_ButNeitherLocalNorUtcWasCalled_ShouldThrow()
    {
        _builder.On(1990, 12, 2);

        var build = () => _builder.Build(out _);

        build.Should().Throw<PointInTimeBuilderNeitherLocalNorUtcException>().WithMessage("Call either InLocalZone() or InUtc() when configuring the point in time.");
    }

    [Fact]
    public void Build_AtWasCalled_ButNeitherLocalNorUtcWasCalled_ShouldThrow()
    {
        _builder.At(14, 0, 0);

        var build = () => _builder.Build(out _);

        build.Should().Throw<PointInTimeBuilderNeitherLocalNorUtcException>().WithMessage("Call either InLocalZone() or InUtc() when configuring the point in time.");
    }

    [Fact]
    public void InTheFuture_ShouldReturnPointOfTimeInTheFuture()
    {
        _builder.InTheFuture();

        _builder.Build(out var kind).DateTimeUtc.Should().BeAfter(DateTime.UtcNow);
        kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void InTheFuture_WithLocalTime_ShouldReturnPointOfTimeInTheFutureInLocalTime()
    {
        _builder.InTheFuture().InLocalZone();

        _builder.Build(out var kind).DateTimeUtc.Should().BeAfter(DateTime.UtcNow);
        kind.Should().Be(DateTimeKind.Local);
    }

    [Fact]
    public void InThePast_ShouldReturnPointOfTimeInThePast()
    {
        _builder.InThePast();

        _builder.Build(out var kind).DateTimeUtc.Should().BeBefore(DateTime.UtcNow);
        kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void On_ShouldReturnSetDateAndCurrentTime()
    {
        _builder
            .On(1990, 12, 2)
            .InLocalZone();

        var now = DateTime.Now;

        _builder.Build(out var kind).DateTimeUtc.ToLocalTime().Should().BeCloseTo(new DateTime(1990, 12, 2,
            now.Hour, now.Minute, now.Second, now.Millisecond, DateTimeKind.Local), DateTimeComparisonPrecision);
        kind.Should().Be(DateTimeKind.Local);
    }

    [Fact]
    public void At_ShouldReturnSetTimeAndCurrentDate()
    {
        _builder
            .At(14, 15, 30, 893)
            .InLocalZone();

        var now = DateTime.Now;

        _builder.Build(out var kind).DateTimeUtc.ToLocalTime().Should().BeCloseTo(new DateTime(now.Year, now.Month, now.Day,
            14, 15, 30, 893, DateTimeKind.Local), DateTimeComparisonPrecision);
        kind.Should().Be(DateTimeKind.Local);
    }
}