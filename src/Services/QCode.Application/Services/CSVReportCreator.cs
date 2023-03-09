using FluentValidation;
using Microsoft.Extensions.Logging;
using QCode.Application.Common.Models;
using System.IO.Abstractions;
using System.Text;

namespace QCode.Application.Services
{
    public class CSVReportCreator : BaseReportCreator
    {
        public StringBuilder Builder { get; set; }
        protected override string? ContentType => "text/csv";
        protected override string? Extension => "csv";

        public CSVReportCreator(ILogger<CSVReportCreator> logger, 
            IFileSystem fileSystem,
            IValidator<ReportRequest> validator) 
            : base(logger, fileSystem, validator) 
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

        protected override string GetContent()
        {
            return Builder.ToString();
        }
    }
}
