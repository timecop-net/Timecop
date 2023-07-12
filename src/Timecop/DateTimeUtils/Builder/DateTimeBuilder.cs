using System;

namespace TCop.DateTimeUtils;

public class DateTimeBuilder
{
    private readonly DateTimeBuilderContext _context = new DateTimeBuilderContext();

    public DateTimeBuilder At(int hour, int minute, int second, int millisecond = 0)
    {
        _context.Time = new TimePart(hour, minute, second, millisecond);
        return this;
    }

    public DateTimeBuilder On(int year, int month, int day)
    {
        _context.Date = new DatePart(year, month, day);
        return this;
    }

    public DateTimeBuilder LocalTime()
    {
        _context.Kind = DateTimeKind.Local;
        return this;
    }

    public DateTimeBuilder UtcTime()
    {
        _context.Kind = DateTimeKind.Utc;
        return this;
    }

    public DateTime Build()
    {
        if (_context.Kind == null || _context.Kind == DateTimeKind.Unspecified)
        {
            throw new DateTimeKindNotSpecifiedException();
        }

        var now = _context.Kind == DateTimeKind.Local ? DateTime.Now : DateTime.UtcNow;

        _context.Date ??= new DatePart(now.Year, now.Month, now.Day);
        _context.Time ??= new TimePart(now.Hour, now.Minute, now.Second, now.Millisecond);

        return new DateTime(_context.Date.Year, _context.Date.Month, _context.Date.Day,
            _context.Time.Hour, _context.Time.Minute, _context.Time.Second, _context.Time.Millisecond, _context.Kind.Value);
    }
}
