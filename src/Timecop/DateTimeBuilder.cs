using System;

namespace TCop;


public class DatePart
{
    public int Year { get; }
    public int Month { get; }
    public int Day { get; }

    public DatePart(int year, int month, int day)
    {
        Year = year;
        Month = month;
        Day = day;
    }
}

public class TimePart
{
    public int Hour { get; }
    public int Minute { get; }
    public int Second { get; }

    public TimePart(int hour, int minute, int second)
    {
        Hour = hour;
        Minute = minute;
        Second = second;
    }
}

public class DateTimeBuilderContext
{
    public DatePart? Date { get; set; }
    public TimePart? Time { get; set; }

    public DateTimeKind? Kind { get; set; }
}

public class DateTimeBuilder
{
    private readonly DateTimeBuilderContext _context = new DateTimeBuilderContext();

    public DateTimeBuilder()
    {
    }

    public DateTimeBuilder At(int hour, int minute, int second)
    {
        _context.Time = new TimePart(hour, minute, second);
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
            throw new InvalidDateTimeKindException();
        }

        var now = _context.Kind == DateTimeKind.Local ? DateTime.Now : DateTime.UtcNow;

        _context.Date ??= new DatePart(now.Year, now.Month, now.Day);
        _context.Time ??= new TimePart(now.Hour, now.Minute, now.Second);

        return new DateTime(_context.Date.Year, _context.Date.Month, _context.Date.Day,
            _context.Time.Hour, _context.Time.Minute, _context.Time.Second, _context.Kind.Value);
    }
}
