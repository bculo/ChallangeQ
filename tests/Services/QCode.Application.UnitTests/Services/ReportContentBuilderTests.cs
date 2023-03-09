using FluentAssertions;
using QCode.Application.Services;

namespace QCode.Application.UnitTests.Services
{
    public class ReportContentBuilderTests
    {
        [Theory]
        [InlineData(null, 10)]
        [InlineData("", 10)]
        [InlineData("TESTVALUEHEADER", 10)]
        public void AddHeaderItem_ShouldReturnBuilderInstance_WhenAnyValueProvided(string? value, int width)
        {
            var builder = new ReportContentBuilder(string.Empty, string.Empty);

            var result = builder.AddHeaderItem(value, width);

            result.Should().NotBeNull();
        }

        [Fact]
        public void AddBodyRow_ShouldReturnBuilderInstance_WhenInvoked()
        {
            var builder = new ReportContentBuilder(string.Empty, string.Empty);

            var result = builder.AddBodyRow();

            result.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null, 10)]
        [InlineData("", 10)]
        [InlineData("TESTVALUEBODYITEM", 10)]
        public void AddBodyRowItem_ShouldThrowArgumentNullException_WhenAnyValueProvided(string? value, int width)
        {
            var builder = new ReportContentBuilder(string.Empty, string.Empty);

            Action action = () => builder.AddBodyRowItem(value, width);

            action.Should().Throw<ArgumentNullException>(); 
        }

        [Fact]
        public void Build_ShouldReturnReportModel_WhenInvoked()
        {
            var builder = new ReportContentBuilder(string.Empty, string.Empty);

            var result = builder.Build();

            result.Should().NotBeNull();
        }
    }
}
