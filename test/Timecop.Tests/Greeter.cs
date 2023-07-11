
using FluentAssertions;
using TCop;

namespace PostExamples
{
    
    public class Greeter
    {
        public string Greet()
        {
            var timeOfDay = Clock.Now.Hour switch
            {
                >= 0 and < 6 => "night",
                >= 6 and < 12 => "morning",
                >= 12 and < 18 => "afternoon",
                _ => "evening"
            };

            return $"Good {timeOfDay}!";
        }

        [Fact]

        public void ThreeAm_ShouldSayGoodNight()
        {
            using var tc = Timecop.Frozen(o => o.At(14, 0,0).LocalTime());

            Greet().Should().Be("Good afternoon!");

            tc.TravelBy(TimeSpan.FromHours(6)); // travel to 8pm local time

            Greet().Should().Be("Good evening!");
        }
    }
}