using FluentValidation;
using Microsoft.Extensions.Logging;
using QCode.Application.Common.Models;
using System.IO.Abstractions;
using System.Text;

namespace QCode.Application.Services
{
    public class TxtReportCreator : BaseReportCreator
    {
        public StringBuilder Builder { get; set; }
        public string RightMargin => "\t";
        protected override string? ContentType => "text/plain";
        protected override string? Extension => "txt";

        public TxtReportCreator(ILogger<TxtReportCreator> logger, 
            IFileSystem fileSystem,
            IValidator<ReportRequest> validator)
            : base(logger, fileSystem, validator)
        {
            Builder = new StringBuilder();
        }

        protected override Task PrepareContent()
        {
            Builder.Clear();

            AppendRowItemsToStringBuilder(Request!.Header!.Items!);

            foreach (var bodyRow in Request!.Body!)
            {
                AppendRowItemsToStringBuilder(bodyRow!.Items!);
            }

            return Task.CompletedTask;
        }

        private void AppendRowItemsToStringBuilder(IEnumerable<ReportRowItem> rowItems)
        {
            foreach (var item in rowItems ?? Enumerable.Empty<ReportRowItem>())
            {
                Builder.Append(FormatRowItem(item!));
            }

            Builder.Append(Environment.NewLine);
        }

        private string FormatRowItem(ReportRowItem item)
        {
            string text = item.Text?.Trim() ?? string.Empty;

            if (text.Length < item.Width && item.Alignment == ReportRowItemAlignment.RIGHT)
            {
                return $"{text.PadLeft(item.Width, ' ')}{RightMargin}";
            }

            if (text.Length < item.Width && item.Alignment == ReportRowItemAlignment.LEFT)
            {
                return $"{text.PadRight(item.Width, ' ')}{RightMargin}";
            }

            if (text.Length > item.Width)
            {
                return $"{text.Substring(0, item.Width)}{RightMargin}";
            }

            return $"{text}{RightMargin}";
        }

        protected override string GetContent()
        {
            return Builder.ToString();
        }
    }
}
