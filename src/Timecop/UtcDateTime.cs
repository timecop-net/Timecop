using System;

namespace TCop;

public struct UtcDateTime
{
    public readonly DateTime UtcValue;

    public UtcDateTime(DateTime value)
    {
        if (value.Kind == DateTimeKind.Unspecified)
        {
            throw new InvalidDateTimeKindException();
        }

        UtcValue = value.ToUniversalTime();
    }
}

public class InvalidDateTimeKindException : Exception
{
    public InvalidDateTimeKindException(): base("DateTimeKind.Unspecified is not supported. Use DateTimeKind.Utc or DateTimeKind.Local.")
    {
    }
}