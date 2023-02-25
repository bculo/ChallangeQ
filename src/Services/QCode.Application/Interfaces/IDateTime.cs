namespace QCode.Application.Interfaces
{
    public interface IDateTime
    {
        DateTime Utc { get; }
        DateTime Date { get; }
        DateTime DateTime { get; }
        public string? TimeZoneName { get; }
        public MomentOfTheDayInfo DayInfo { get; }
    }

    public class MomentOfTheDayInfo
    {
        public DateTime Current { get; private set; }
        public DateTime StartOfTheDay { get; private set; }
        public DateTime EndOfTheDay { get; private set; }

        public MomentOfTheDayInfo(DateTime current, DateTime starts, DateTime ends)
        {
            Current = current;
            StartOfTheDay = starts;
            EndOfTheDay = ends;
        }
    }
}
