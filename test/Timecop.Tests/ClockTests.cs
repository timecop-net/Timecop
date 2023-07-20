using FluentAssertions;

namespace TCop.Tests
{
    public class ClockTests
    {
        private static readonly TimeSpan DateTimeComparisonPrecision = TimeSpan.FromMilliseconds(50);

        [Fact]
        public void UtcNow_NoTimecopCreated_ShouldReturnCurrentTime()
        {
            Clock.UtcNow.Should().BeCloseTo(DateTime.UtcNow, DateTimeComparisonPrecision);
        }

        [Fact]
        public void UtcNow_TimecopWithoutTravel_ShouldReturnCurrentTime()
        {
            using var tc = new Timecop();
            Clock.UtcNow.Should().BeCloseTo(DateTime.UtcNow, DateTimeComparisonPrecision);
        }
    }
}