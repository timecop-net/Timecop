using System;

namespace TCop.Time.Builder;

public class PointInTimeBuilderNeitherLocalNorUtcException : Exception
{
    public PointInTimeBuilderNeitherLocalNorUtcException() : base($"Call either {nameof(PointInTimeBuilder.InLocalZone)}() or {nameof(PointInTimeBuilder.InUtc)}() when configuring the point in time.")
    {
    }
}