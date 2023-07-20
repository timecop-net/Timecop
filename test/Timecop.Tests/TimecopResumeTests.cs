using FluentAssertions;

namespace TCop.Tests
{
    public class TimecopResumeTests
    {
        private static readonly TimeSpan DateTimeComparisonPrecision = TimeSpan.FromMilliseconds(50);

        [Fact]
        public void Resume_ShouldResumeFrozenTime_AndReturnCurrentUtcTime()
        {
            using var tc = new Timecop();
            var frozenAt = tc.Freeze();
            
            Thread.Sleep(200);
            
            var resumedTime = tc.Resume();

            Thread.Sleep(200);

            resumedTime.Should().Be(frozenAt);

            Clock.UtcNow.Should().BeCloseTo(frozenAt.AddMilliseconds(200), DateTimeComparisonPrecision);
        }
    }
}