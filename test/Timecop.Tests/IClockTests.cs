using FluentAssertions;

namespace TCop.Tests
{
    public class IClockTests
    {
        private static readonly TimeSpan DateTimeComparisonPrecision = TimeSpan.FromMilliseconds(50);

        [Fact]
        public void UtcNow_TimecopWithTravel_ShouldReturnCurrentTime()
        {
            var fiveHours = TimeSpan.FromHours(5);

            using var tc = new Timecop();
            tc.TravelBy(fiveHours);

            tc.Clock.UtcNow.Should().BeCloseTo(DateTime.UtcNow.Add(fiveHours), DateTimeComparisonPrecision);
        }

        [Fact]
        public void Clock_TwoClocksShouldBeTheSameInstance()
        {
            using var tc = new Timecop();

            tc.Clock.Should().Be(tc.Clock);
        }
    }
}