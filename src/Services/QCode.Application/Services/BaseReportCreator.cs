﻿using FluentValidation;
using Microsoft.Extensions.Logging;
using QCode.Application.Common.Models;
using QCode.Application.Interfaces;
using QCode.Core.Exceptions;

namespace QCode.Application.Services
{
    public abstract class BaseReportCreator : IReportCreator
    {
        protected readonly ILogger _logger;
        protected ReportRequest? Request { get; set; }
        protected string? FullFilePath { get; set; }

        protected abstract string? ContentType { get; }
        protected abstract string? Extension { get; }

        public BaseReportCreator(ILogger logger)
        {
            _logger = logger;
        }

        protected abstract Task PrepareContent();
        protected abstract Task SaveReport();

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
            var validator = new ReportRequestValidator();
            var validationResult = await validator.ValidateAsync(Request!);

            if(validationResult.Errors.Any(i => i.PropertyName == "SaveToLocation"))
            {
                throw new QCodeCriticalException("Invalid location path for storing files");
            }

            if(!validationResult.IsValid)
            {
                throw new QCodeValidationException(validationResult.ToDictionary());
            }
        }

        protected virtual void DefineFullFilePath()
        {
            var pathWithoutExtension = Path.Combine(Request!.SaveToLocation!, Request!.FileName!);
            FullFilePath = Path.ChangeExtension(pathWithoutExtension, Extension);

            if(string.IsNullOrEmpty(FullFilePath))
            {
                throw new QCodeValidationException("File path not valid!");
            }
        }
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
}
