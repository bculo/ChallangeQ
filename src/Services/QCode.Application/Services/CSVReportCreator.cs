using Microsoft.Extensions.Logging;
using QCode.Application.Common.Models;
using QCode.Core.Exceptions;
using System.Security;
using System.Text;

namespace QCode.Application.Services
{
    public class CSVReportCreator : BaseReportCreator
    {
        public StringBuilder Builder { get; set; }
        protected override string? ContentType => "text/csv";
        protected override string? Extension => "csv";

        public CSVReportCreator(ILogger<CSVReportCreator> logger) : base(logger) 
        {
            Builder = new StringBuilder();
        }

        protected override Task PrepareContent()
        {
            Builder.Clear();

            AppendRowToStringBuilder(Request!.Header!);

            foreach(var bodyRow in Request!.Body!)
            {
                AppendRowToStringBuilder(bodyRow);
            }

            return Task.CompletedTask;
        }

        private void AppendRowToStringBuilder(ReportRow row)
        {
            var rowFormat = string.Join(",", row?.Items?.Select(i => i.Text ?? string.Empty) ?? Enumerable.Empty<string>());
            Builder.Append(rowFormat);
            Builder.Append(Environment.NewLine);
        }

        protected override async Task SaveReport()
        {
            try
            {
                DefineFullFilePath();
                await File.WriteAllTextAsync(FullFilePath!, Builder.ToString());
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
