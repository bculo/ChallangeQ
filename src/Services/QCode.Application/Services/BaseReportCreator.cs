using FluentValidation;
using Microsoft.Extensions.Logging;
using QCode.Application.Common.Models;
using QCode.Application.Interfaces;
using QCode.Core.Exceptions;
using System.IO.Abstractions;

namespace QCode.Application.Services
{
    public abstract class BaseReportCreator : IReportCreator
    {
        protected readonly ILogger _logger;
        protected readonly IFileSystem _fileSystem;
        protected readonly IValidator<ReportRequest> _validator;

        protected ReportRequest? Request { get; set; }
        protected string? FullFilePath { get; set; }

        protected abstract string? ContentType { get; }
        protected abstract string? Extension { get; }

        public BaseReportCreator(ILogger logger, 
            IFileSystem fileSystem,
            IValidator<ReportRequest> validator)
        {
            _logger = logger;
            _fileSystem = fileSystem;
            _validator = validator;
        }

        protected abstract Task PrepareContent();
        protected abstract string GetContent();

        public async Task<string> CreateReport(ReportRequest request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));

            await ValidateInputModel();
            await PrepareContent();
            await SaveReport();

            return FullFilePath!;
        }

        protected virtual async Task ValidateInputModel()
        {
            var validationResult = await _validator.ValidateAsync(Request!);

            if(!validationResult.IsValid)
            {
                throw new QCodeValidationException(validationResult.ToDictionary());
            }
        }

        protected virtual void DefineFullFilePath()
        {
            var pathWithoutExtension = _fileSystem.Path.Combine(Request!.SaveToLocation!, Request!.FileName!);
            FullFilePath = _fileSystem.Path.ChangeExtension(pathWithoutExtension, Extension);

            if (string.IsNullOrEmpty(FullFilePath))
            {
                throw new QCodeValidationException("File path not valid!");
            }
        }

        protected virtual async Task SaveReport()
        {
            DefineFullFilePath();
            await _fileSystem.File.WriteAllTextAsync(FullFilePath!, GetContent());
        }
    }

    public class ReportRequestValidator : AbstractValidator<ReportRequest>
    {
        public ReportRequestValidator(IFileSystem file)
        {
            RuleFor(i => i.FileName).NotEmpty();
            RuleFor(i => i.Header).NotNull();
            RuleFor(i => i.Body).NotEmpty();
            RuleFor(i => i.SaveToLocation)
                .Must((i) => !file.Path.HasExtension(i) && file.Directory.Exists(i))
                .WithMessage("Invalid file path")
                .When(i => !string.IsNullOrEmpty(i.SaveToLocation))
                .NotEmpty();
        }
    }
}
