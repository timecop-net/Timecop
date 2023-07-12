using System;

namespace TCop.DateTimeUtils
{
    internal class DateTimeBuilderContext
    {
        public DatePart? Date { get; set; }
        public TimePart? Time { get; set; }

        public DateTimeKind? Kind { get; set; }
    }
}