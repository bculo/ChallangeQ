using Microsoft.Extensions.Logging;
using System.Text;

namespace QCode.Application.Services
{
    public class CSVReportCreator : BaseReportCreator
    {
        public StringBuilder Builder { get; set; }
        public string RightMargin => "\t";
        protected override string? ContentType => "text/csv";
        protected override string? Extension => "csv";

        public CSVReportCreator(ILogger<CSVReportCreator> logger) : base(logger) 
        {
            Builder = new StringBuilder();
        }

        protected override Task PrepareContent()
        {
            Builder.Clear();

            AppendRowItemsToStringBuilder(Request!.Header!.Items!);

            foreach(var bodyRow in Request!.Body!)
            {
                AppendRowItemsToStringBuilder(bodyRow!.Items!);
            }

            return Task.CompletedTask;
        }

        private void AppendRowItemsToStringBuilder(IEnumerable<ReportRowItem> rowItems)
        {
            foreach(var item in rowItems ?? Enumerable.Empty<ReportRowItem>())
            {
                Builder.Append(FormatRowItem(item!));
            }

            Builder.Append(Environment.NewLine);
        }

        private string FormatRowItem(ReportRowItem item)
        {
            string text = item.Text?.Trim() ?? string.Empty;

            if(text.Length < item.Width && item.Alignment == ReportRowItemAlignment.RIGHT)
            {
               return $"{text.PadLeft(item.Width, ' ')}{RightMargin}";
            }

            if (text.Length < item.Width && item.Alignment == ReportRowItemAlignment.LEFT)
            {
                return $"{text.PadRight(item.Width, ' ')}{RightMargin}";
            }

            if(text.Length > item.Width)
            {
                return $"{text.Substring(0, item.Width)}{RightMargin}";
            }

            return $"{text}{RightMargin}";
        }

        public string GetRightMargin()
        {
            return "\t";
        }

        protected override async Task SaveReport()
        {
            await File.WriteAllTextAsync(GetFullFilePath(), Builder.ToString());
        }
    }
}
