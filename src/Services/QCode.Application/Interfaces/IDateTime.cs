using QCode.Application.Common.Models;

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
}
