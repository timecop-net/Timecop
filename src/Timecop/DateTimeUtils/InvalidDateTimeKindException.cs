using System;

namespace TCop.DateTimeUtils
{
    public class InvalidDateTimeKindException : Exception
    {
        public InvalidDateTimeKindException() : base("DateTimeKind.Unspecified is not supported. Use DateTimeKind.Utc or DateTimeKind.Local.")
        {
        }
    }
}