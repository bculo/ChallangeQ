using FluentAssertions;
using QCode.Application.Common.Models;
using QCode.Application.Services;

namespace QCode.Application.UnitTests.Services
{
    public class UKLocalTimeServiceTests
    {
        private readonly UKLocalTimeService _timeService = new UKLocalTimeService();

        public UKLocalTimeServiceTests()
        {
            _timeService = new UKLocalTimeService();
        }

        [Fact]
        public void DateTime_ShouldReturnUnspecifiedKind_WhenPropertyInvoked()
        {
            var time = _timeService.DateTime;

            time.Kind.Should().Be(DateTimeKind.Unspecified);
        }

        [Fact]
        public void TimeZoneName_ShouldReturnGMTzone_WhenPropertyInvoked()
        {
            var timezone = _timeService.TimeZoneName;

            timezone.Should().NotBeNull()
                .And.Be("GMT Standard Time");
        }

        [Fact]
        public void Date_ShouldReturnDateTimeInstanceWithResetedTimePart_WhenPropertyInvoked()
        {
            var date = _timeService.Date;

            date.Minute.Should().Be(0);
            date.Hour.Should().Be(0);
            date.Second.Should().Be(0);
            date.Millisecond.Should().Be(0);
        }

        [Fact]
        public void DayInfo_ShouldReturnMomentOfTheDayInfoInstance_WhenPropertyInvoked()
        {
            var info = _timeService.DayInfo;

            info.Should().NotBeNull()
                .And.BeOfType<MomentOfTheDayInfo>();
        }
    }
}
