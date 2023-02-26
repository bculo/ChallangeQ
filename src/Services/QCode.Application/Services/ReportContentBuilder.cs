using QCode.Application.Common.Models;

namespace QCode.Application.Services
{
    public class ReportContentBuilder
    {
        private ReportRequest _model;
        private ReportRow? _lastRow { get; set; }

        public ReportContentBuilder(string? fileName, string? fileLocation)
        {
            ArgumentNullException.ThrowIfNull(fileName);
            ArgumentNullException.ThrowIfNull(fileLocation);

            _model = new ReportRequest
            {
                FileName = fileName,
                SaveToLocation = fileLocation,
                Body = new List<ReportRow>(),
                Header = new ReportRow()
            };

            _model.Header.Items = new List<ReportRowItem>();
        }

        public ReportContentBuilder AddHeaderItem(string value, int columnWidth, ReportRowItemAlignment alignment = ReportRowItemAlignment.LEFT)
        {
            _model.Header!.Items!.Add(new ReportRowItem
            {
                Text = value?.Trim() ?? string.Empty,
                Alignment = alignment,
                Width = columnWidth
            });

            return this;
        }

        public ReportContentBuilder AddBodyRow()
        {
            _lastRow = new ReportRow
            {
                Items = new List<ReportRowItem>()
            };
            _model.Body!.Add(_lastRow);
            return this;
        }

        public ReportContentBuilder AddBodyRowItem(string value, int columnWidth, ReportRowItemAlignment alignment = ReportRowItemAlignment.LEFT)
        {
            _lastRow!.Items!.Add(new ReportRowItem
            {
                Text = value?.Trim() ?? string.Empty,
                Alignment = alignment,
                Width = columnWidth
            });

            return this;
        }

        public ReportRequest Build()
        {
            return _model;
        }
    }
}
