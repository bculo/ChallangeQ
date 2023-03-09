using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using QCode.Application.Common.Models;
using QCode.Application.Services;
using QCode.Core.Exceptions;
using System.IO.Abstractions;

namespace QCode.Application.UnitTests.Services
{
    public class CSVReportCreatorTests : BaseReportCreatorTests
    {
        private readonly ILogger<CSVReportCreator> _logger = Substitute.For<ILogger<CSVReportCreator>>();

        public CSVReportCreatorTests() : base() { }

        public override BaseReportCreator CreateInstance()
        {
            return new CSVReportCreator(_logger, _fileSystem, _validator);
        }

        public override string GetExtension()
        {
            return ".csv";
        }
    }
}
