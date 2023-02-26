using QCode.Application.Common.Models;
using QCode.Application.Interfaces;

namespace QCode.Application.Services
{
    public class UKLocalTimeService : IDateTime
    {
        private readonly TimeZoneInfo _timeZoneUK;

        public UKLocalTimeService()
        {
            _timeZoneUK = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        }

        public DateTime Date => GetUKDateTime().Date;

        public DateTime DateTime => GetUKDateTime();

        public DateTime Utc => DateTime.UtcNow;

        public string? TimeZoneName => _timeZoneUK.StandardName;

        public MomentOfTheDayInfo DayInfo => GetDayInformation();

        private DateTime GetUKDateTime()
        {
            return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, _timeZoneUK);
        }

        private MomentOfTheDayInfo GetDayInformation()
        {
            var currentDateTime = GetUKDateTime();
            var currentDate = currentDateTime.Date;
            var dayStarts = currentDate.Date.AddHours(-1);
            var dayEnds = currentDate.Date.AddHours(23);

            return new MomentOfTheDayInfo(currentDateTime, dayStarts, dayEnds);
        }
    }
}
