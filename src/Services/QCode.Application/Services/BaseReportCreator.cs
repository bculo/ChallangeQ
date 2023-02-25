using FluentValidation;
using Microsoft.Extensions.Logging;
using QCode.Application.Interfaces;
using QCode.Core.Exceptions;

namespace QCode.Application.Services
{
    public abstract class BaseReportCreator : IReportCreator
    {
        protected readonly ILogger _logger;
        protected ReportRequest? Request { get; set; }

        protected abstract string? ContentType { get; }
        protected abstract string? Extension { get; }

        public BaseReportCreator(ILogger logger)
        {
            _logger = logger;
        }

        protected abstract Task PrepareContent();
        protected abstract Task SaveReport();

        public async Task CreateReport(ReportRequest request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));

            await ValidateInputModel();
            await PrepareContent();
            await SaveReport();
        }

        protected virtual async Task ValidateInputModel()
        {
            _logger.LogTrace("Validation started");

            var validator = new ReportRequestValidator();
            var validationResult = await validator.ValidateAsync(Request!);

            if(!validationResult.IsValid)
            {
                throw new QCodeValidationException(validationResult.ToDictionary());
            }

            _logger.LogTrace("Validation finished");
        }

        protected virtual string GetFullFilePath()
        {
            var pathWithoutExtension = Path.Combine(Request!.SaveToLocation!, Request!.FileName!);
            var fullPath = Path.ChangeExtension(pathWithoutExtension, Extension);
            if(string.IsNullOrEmpty(fullPath))
            {
                throw new QCodeValidationException("File path not valid!");
            }
            return fullPath;
        }

    }

    public class ReportRequest
    {
        public string? FileName { get; set; }
        public string? SaveToLocation { get; set; }
        public ReportRow? Header { get; set; }
        public List<ReportRow>? Body { get; set; }
    }

    public class ReportRequestValidator : AbstractValidator<ReportRequest>
    {
        public ReportRequestValidator()
        {
            RuleFor(i => i.FileName).NotEmpty();
            RuleFor(i => i.Header).NotNull();
            RuleFor(i => i.Body).NotEmpty();
            RuleFor(i => i.SaveToLocation)
                .Must((i) => !Path.HasExtension(i) && Directory.Exists(i))
                .WithMessage("Invalid file path")
                .When(i => !string.IsNullOrEmpty(i.SaveToLocation))
                .NotEmpty();
        }
    }

    public class ReportRow
    {
        public List<ReportRowItem>? Items { get; set; }
    }

    public class ReportRowItem
    {
        public int Width { get; set; }
        public string? Text { get; set; }
        public ReportRowItemAlignment Alignment { get; set; }
    }

    public enum ReportRowItemAlignment
    {
        LEFT = 0,
        RIGHT = 1,
    }

    public class ReportResponse
    {
        public string? Base64Content { get; set; }
        public string? ContentType { get; set; }
        public string? Extension { get; set; }
        public string? Name { get; set; }
    }
}
