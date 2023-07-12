using FluentAssertions;
using TCop.DateTimeUtils;

namespace TCop.Tests;

public class UtcDateTimeTests
{
    [Fact]
    public void UtcValue_GivenUtcDateTime_ShouldNotConvert()
    {
        var utcValue = new DateTime(1990, 12, 2, 0, 0, 0, DateTimeKind.Utc);

        var utcDateTime = new UtcDateTime(utcValue);

        utcDateTime.UtcValue.Should().Be(utcValue);
    }

    [Fact]
    public void UtcValue_GivenLocalDateTime_ShouldConvertToUtc()
    {
        TimeZoneInfo.Local.BaseUtcOffset.Should().NotBe(TimeSpan.Zero);
        
        var utcValue = new DateTime(1990, 12, 2, 0, 0, 0, DateTimeKind.Utc);
        var localValue = utcValue.ToLocalTime();

        var utcDateTime = new UtcDateTime(localValue);

        utcDateTime.UtcValue.Should().Be(utcValue);
    }

    [Fact]
    public void UtcValue_GivenUnspecifiedDateTime_ShouldThrowAnException()
    {
        var unspecifiedDateTime = new DateTime(1990, 12, 2, 0, 0, 0, DateTimeKind.Unspecified);

        var createUtcDateTime = () => new UtcDateTime(unspecifiedDateTime);

        createUtcDateTime.Should().Throw<InvalidDateTimeKindException>()
            .WithMessage("DateTimeKind.Unspecified is not supported. Use DateTimeKind.Utc or DateTimeKind.Local.");
    }
}