using FluentAssertions;

namespace TCop.Tests
{
    public class TimecopTravelTests
    {
        private static readonly TimeSpan DateTimeComparisonPrecision = TimeSpan.FromMilliseconds(50);

        [Fact]
        public void TravelBy_InFrozenState_ShouldFreezeAndTravel_AndReturnCurrentUtcTime()
        {
            using var tc = new Timecop();
            var frozenAt = tc.Freeze();
            var traveledTo = tc.TravelBy(TimeSpan.FromDays(3));

            traveledTo.Should().Be(frozenAt.AddDays(3));
        }
    }
}