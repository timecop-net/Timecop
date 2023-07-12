using FluentAssertions;
using TCop.DateTimeUtils;

namespace TCop.Tests
{
    public class DateTimeBuilderTests
    {
        private static readonly TimeSpan DateTimeComparisonPrecision = TimeSpan.FromMilliseconds(50);

        [Fact]
        public void LocalTime_ShouldReturnLocalTime()
        {
            var builder = new DateTimeBuilder();

            builder.LocalTime();

            builder.Build().Should().BeCloseTo(DateTime.Now, DateTimeComparisonPrecision);
        }

        [Fact]
        public void UtcTime_ShouldReturnLocalTime()
        {
            var builder = new DateTimeBuilder();

            builder.UtcTime();

            builder.Build().Should().BeCloseTo(DateTime.UtcNow, DateTimeComparisonPrecision);
        }

        [Fact]
        public void Build_NeitherLocalNorUtcWasCalled_ShouldThrow()
        {
            var builder = new DateTimeBuilder();

            var build = () => builder.Build();
            build.Should().Throw<DateTimeKindNotSpecifiedException>().WithMessage("Specify either LocalTime() or UtcTime() when configuring time to freeze.");
        }

        [Fact]
        public void On_ShouldReturnSetDateAndCurrentTime()
        {
            var builder = new DateTimeBuilder();

            builder
                .On(1990, 12, 2)
                .LocalTime();

            var now = DateTime.Now;

            builder.Build().Should().BeCloseTo(new DateTime(1990, 12, 2, 
                now.Hour, now.Minute, now.Second, now.Millisecond, DateTimeKind.Local), DateTimeComparisonPrecision);
        }

        [Fact]
        public void At_ShouldReturnSetTimeAndCurrentDate()
        {
            var builder = new DateTimeBuilder();

            builder
                .At(14, 15, 30, 893)
                .LocalTime();

            var now = DateTime.Now;

            builder.Build().Should().BeCloseTo(new DateTime(now.Year, now.Month, now.Day,
                14, 15, 30, 893, DateTimeKind.Local), DateTimeComparisonPrecision);
        }
    }
}