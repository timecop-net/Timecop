namespace TCop.Time.Builder;

internal class DatePart
{
    public int Year { get; }
    public int Month { get; }
    public int Day { get; }

    internal DatePart(int year, int month, int day)
    {
        Year = year;
        Month = month;
        Day = day;
    }
}