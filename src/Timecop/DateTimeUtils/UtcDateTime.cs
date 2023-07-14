using System;
using TCop.Core;

namespace TCop.DateTimeUtils;

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

    public PointInTime PointInTime => new (UtcValue);
}