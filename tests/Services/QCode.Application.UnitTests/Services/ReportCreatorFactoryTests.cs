using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using QCode.Application.Common.Enums;
using QCode.Application.Common.Models;
using QCode.Application.Interfaces;
using QCode.Application.Services;
using QCode.Core.Exceptions;
using System.IO.Abstractions;

namespace QCode.Application.UnitTests.Services
{
    public class ReportCreatorFactoryTests
    {
        private readonly ReportCreatorFactory _factory;
        private readonly IServiceProvider _serviceProvider = Substitute.For<IServiceProvider>();
        private readonly ILogger<ReportCreatorFactory> _logger = Substitute.For<ILogger<ReportCreatorFactory>>();

        public ReportCreatorFactoryTests()
        {
            _factory = new ReportCreatorFactory(_serviceProvider, _logger);
        }

        [Fact]
        public void CreateFileCreator_ShouldReturnCsvCreatorInstance_WhenCsvFileTypeProvided()
        {
            _serviceProvider.GetService(typeof(CSVReportCreator))
                .Returns(new CSVReportCreator(Substitute.For<ILogger<CSVReportCreator>>(), Substitute.For<IFileSystem>(), Substitute.For<IValidator<ReportRequest>>()));

            var creator = _factory.CreateFileCreator(FileType.CSV);

            creator.Should().NotBeNull()
                .And.BeOfType<CSVReportCreator>()
                .And.BeAssignableTo<IReportCreator>();
        }

        [Fact]
        public void CreateFileCreator_ShouldReturnTxtCreatorInstance_WhenTxtFileTypeProvided()
        {
            _serviceProvider.GetService(typeof(TxtReportCreator))
                .Returns(new TxtReportCreator(Substitute.For<ILogger<TxtReportCreator>>(), Substitute.For<IFileSystem>(), Substitute.For<IValidator<ReportRequest>>()));

            var creator = _factory.CreateFileCreator(FileType.TXT);

            creator.Should().NotBeNull()
                .And.BeOfType<TxtReportCreator>()
                .And.BeAssignableTo<IReportCreator>();
        }

        [Theory]
        [InlineData(FileType.CSV, typeof(CSVReportCreator))]
        [InlineData(FileType.TXT, typeof(TxtReportCreator))]
        public void CreateFileCreator_ShouldThrowArgumentNullException_WhenFileTypeSupportedButCreatorInstanceNotFound(FileType type, Type instance)
        {
            _serviceProvider.GetService(instance).Returns(null);

            Action action = () => _factory.CreateFileCreator(type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateFileCreator_ShouldThrowQCodeBaseException_WhenFileTypeNotSupported()
        {
            Action action = () => _factory.CreateFileCreator(FileType.EXCEL);

            action.Should().Throw<QCodeBaseException>();
        }
    }
}