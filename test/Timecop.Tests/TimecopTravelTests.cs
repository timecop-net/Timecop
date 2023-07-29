using FluentAssertions;
using System.Reflection.Metadata;

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

        [Fact]
        public void TravelToDateTime_ShouldTravelToSpecifiedDateTime_AndReturnThatDateTime()
        {
            using var tc = new Timecop();

            var travelTo = DateTime.Now.AddDays(-7);

            var traveledTo = tc.TravelTo(travelTo);

            Thread.Sleep(100);

            Clock.Now.Should().BeCloseTo(
                traveledTo.Add(TimeSpan.FromMilliseconds(100)), DateTimeComparisonPrecision);

            traveledTo.Should().Be(travelTo);
        }

        [Fact]
        public void TravelToLocalDateAndTime_ShouldTravelToGivenDateAndTime_AndReturnLocalTime()
        {
            using var tc = new Timecop();

            var traveledTo = tc.TravelTo(1990, 12, 2, 14, 34, 55, DateTimeKind.Local);
            var expectedTraveledTo = new DateTime(1990, 12, 2, 14, 34, 55, DateTimeKind.Local);

            Thread.Sleep(100);

            Clock.Now.Should().BeCloseTo(
                traveledTo.Add(TimeSpan.FromMilliseconds(100)), DateTimeComparisonPrecision);
            traveledTo.Should().Be(expectedTraveledTo);
        }

        [Fact]
        public void TravelToLocalDate_ShouldTravelToGivenDate_AndReturnLocalDateTimeTraveledTo()
        {
            using var tc = new Timecop();

            var traveledTo = tc.TravelTo(1990, 12, 2, DateTimeKind.Local);
            var expectedTraveledTo = new DateTime(1990, 12, 2, 0, 0, 0, DateTimeKind.Local);

            Thread.Sleep(100);

            Clock.Now.Should().BeCloseTo(
                traveledTo.Add(TimeSpan.FromMilliseconds(100)), DateTimeComparisonPrecision);
            traveledTo.Should().Be(expectedTraveledTo);
        }

        [Fact]
        public void TravelToRandomFutureLocalTime_ShouldTravel_AndReturnTheLocalDateTimeTraveledTo()
        {
            using var tc = new Timecop();

            var traveledTo = tc.TravelTo(o => o.InTheFuture().LocalTime());
            var currentLocalTime = DateTime.Now;

            Thread.Sleep(100);

            Clock.Now.Should().BeCloseTo(
                traveledTo.Add(TimeSpan.FromMilliseconds(100)), DateTimeComparisonPrecision);
            traveledTo.Should().BeAfter(currentLocalTime);
        }
    }
}