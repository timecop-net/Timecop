using System;

namespace TCop.DateTimeUtils
{
    public class DateTimeKindNotSpecifiedException : Exception
    {
        public DateTimeKindNotSpecifiedException() : base($"Specify either {nameof(DateTimeBuilder.LocalTime)}() or {nameof(DateTimeBuilder.UtcTime)}() when configuring time to freeze.")
        {
        }
    }
}