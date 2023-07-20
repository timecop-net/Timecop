using FluentAssertions;

namespace TCop.Tests
{
    public class TimecopFreezeTests
    {
        private static readonly TimeSpan DateTimeComparisonPrecision = TimeSpan.FromMilliseconds(50);

        [Fact]
        public void Freeze_ShouldFreezeAtCurrentTime_AndReturnCurrentUtcTime()
        {
            using var tc = new Timecop();
            var frozenAt = tc.Freeze();
            var realTimeFrozenAt = DateTime.UtcNow;

            Thread.Sleep(100);

            Clock.UtcNow.Should().Be(frozenAt);
            frozenAt.Should().BeCloseTo(realTimeFrozenAt, DateTimeComparisonPrecision);
        }

        [Fact]
        public void FreezeAtLocalDateTime_ShouldFreezeAtSpecifiedTime_AndReturnFrozenLocalTime()
        {
            using var tc = new Timecop();
            var freezeAt = DateTime.Now.AddDays(-7);

            var frozenAt = tc.Freeze(freezeAt);

            Thread.Sleep(100);

            Clock.Now.Should().Be(freezeAt);
            frozenAt.Should().Be(freezeAt);
        }

        [Fact]
        public void FreezeAtLocalDateAndTime_ShouldFreezeAtSpecifiedDateTime_AndReturnFrozenLocalTime()
        {
            using var tc = new Timecop();

            var frozenAt = tc.Freeze(1990, 12, 2, 14, 34, 55, DateTimeKind.Local);
            var expectedFrozenAt = new DateTime(1990, 12, 2, 14, 34, 55, DateTimeKind.Local);

            Thread.Sleep(100);

            Clock.Now.Should().Be(frozenAt);
            frozenAt.Should().Be(expectedFrozenAt);
        }

        [Fact]
        public void FreezeAtLocalDate_ShouldFreezeAtSpecifiedDate_AndReturnFrozenLocalDateTime()
        {
            using var tc = new Timecop();

            var frozenAt = tc.Freeze(1990, 12, 2, DateTimeKind.Local);
            var expectedFrozenAt = new DateTime(1990, 12, 2, 0, 0, 0, DateTimeKind.Local);

            Thread.Sleep(100);

            Clock.Now.Should().Be(frozenAt);
            frozenAt.Should().Be(expectedFrozenAt);
        }

        [Fact]
        public void FreezeAtRandomFutureLocalTime_ShouldFreeze_AndReturnFrozenLocalDateTime()
        {
            using var tc = new Timecop();

            var frozenAt = tc.Freeze(o => o.InTheFuture().LocalTime());
            var currentLocalTime = DateTime.Now;

            Thread.Sleep(100);

            Clock.Now.Should().Be(frozenAt);
            frozenAt.Should().BeAfter(currentLocalTime);
        }
    }
}