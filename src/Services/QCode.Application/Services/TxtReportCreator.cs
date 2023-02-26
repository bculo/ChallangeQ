using Microsoft.Extensions.Logging;
using QCode.Application.Common.Models;
using QCode.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace QCode.Application.Services
{
    public class TxtReportCreator : BaseReportCreator
    {
        public StringBuilder Builder { get; set; }
        public string RightMargin => "\t";
        protected override string? ContentType => "text/plain";
        protected override string? Extension => "txt";

        public TxtReportCreator(ILogger<TxtReportCreator> logger) : base(logger)
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

        protected override async Task SaveReport()
        {
            try
            {
                await File.WriteAllTextAsync(GetFullFilePath(), Builder.ToString());
            }
            catch (SecurityException e) { HandleCriticalException(e); }
            catch (UnauthorizedAccessException e) { HandleCriticalException(e); }
            catch (PathTooLongException e) { HandleCriticalException(e); }
        }

        private void HandleCriticalException(Exception e)
        {
            _logger.LogError(e, e.Message);
            throw new QCodeCriticalException("Problem with writing file to given location");
        }
    }
}
