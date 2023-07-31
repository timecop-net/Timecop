using System;

namespace TCop
{
    public interface IClock
    {
        /// <summary>Returns either current or pre-configured local time. Time can be pre-configured by using <see cref="T:TCop.Timecop" />.</summary>
        DateTime Now { get; }

        /// <summary>Returns either current or pre-configured UTC time. Time can be pre-configured by using <see cref="T:TCop.Timecop" />.</summary>
        DateTime UtcNow { get; }
    }
}