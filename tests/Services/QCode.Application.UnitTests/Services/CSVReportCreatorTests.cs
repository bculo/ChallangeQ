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
    public class CSVReportCreatorTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly ILogger<CSVReportCreator> _logger = Substitute.For<ILogger<CSVReportCreator>>();

        private CSVReportCreator? _creator;
        private IFileSystem _fileSystem = Substitute.For<IFileSystem>();
        private IValidator<ReportRequest> _validator = Substitute.For<IValidator<ReportRequest>>();

        public CSVReportCreatorTests()
        {
            _fileSystem.Path.HasExtension(Arg.Any<string>()).Returns(false);
            _fileSystem.File.WriteAllText(Arg.Any<string>(), Arg.Any<string>());
        }

        [Theory]
        [InlineData("C:\\validpath")]
        [InlineData("C:\\test\\validpath")]
        [InlineData("C:\\validpath\\stonks")]
        [InlineData("D:\\test\\test\\validpath")]
        public async Task CreateReport_ShouldReturnFileLocationWithExtension_WhenReportModelProvided(string systemPath)
        {
            //Arrange
            _fixture.Customize<ReportRequest>(opt =>
            {
                return opt.With(s => s.FileName, "file")
                    .With(s => s.SaveToLocation, systemPath);
            });

            var model = _fixture.Create<ReportRequest>();

            _validator = new ReportRequestValidator(_fileSystem);

            _fileSystem.Directory.Exists(Arg.Is<string>(s => s.Contains("validpath"))).Returns(true);
            _fileSystem.Path.Combine(Arg.Any<string>(), Arg.Any<string>()).Returns($"{systemPath}\\{model.FileName}");
            _fileSystem.Path.ChangeExtension(Arg.Any<string>(), Arg.Any<string>()).Returns($"{systemPath}\\{model.FileName}.csv");

            _creator = new CSVReportCreator(_logger, _fileSystem, _validator);

            //Act
            var result = await _creator.CreateReport(model);

            //Assert
            result.Should().NotBeNull()
                .And.StartWith(systemPath)
                .And.Contain(model.FileName)
                .And.EndWith(".csv");
        }

        [Fact]
        public async Task CreateReport_ShouldThrowArgumentNullException_WhenNullValueProvided()
        {
            //Arrange
            ReportRequest? model = null;
            _creator = new CSVReportCreator(_logger, _fileSystem, _validator);

            //Act
            Func<Task> func = async () => await _creator.CreateReport(model);

            //Assert
            await func.Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("C:\\invalidPath")]
        [InlineData("C:\\!-asd222.32---")]
        public async Task CreateReport_ShouldThrowQCodeValidationException_WhenInvalidSaveLocationProvided(string? systemPath)
        {
            //Arrange
            _fixture.Customize<ReportRequest>(opt =>
            {
                return opt.With(s => s.FileName, "file")
                    .With(s => s.SaveToLocation, systemPath);
            });

            var model = _fixture.Create<ReportRequest>();

            _fileSystem.Directory.Exists(Arg.Any<string>()).Returns(false);

            _validator = new ReportRequestValidator(_fileSystem);

            _creator = new CSVReportCreator(_logger, _fileSystem, _validator);

            //Act
            Func<Task> func = async () => await _creator.CreateReport(model);

            //Assert
            await func.Should().ThrowAsync<QCodeValidationException>()
                .WithMessage("Validation exception")
                .Where(i => i.Errors!.ContainsKey("SaveToLocation"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task CreateReport_ShouldThrowQCodeValidationException_WhenInvalidFileNameProvided(string? fileName)
        {
            //Arrange
            _fixture.Customize<ReportRequest>(opt => opt.With(s => s.FileName, fileName));

            var model = _fixture.Create<ReportRequest>();

            _fileSystem.Directory.Exists(Arg.Any<string>()).Returns(true);
            _fileSystem.Path.HasExtension(Arg.Any<string>()).Returns(false);

            _validator = new ReportRequestValidator(_fileSystem);

            _creator = new CSVReportCreator(_logger, _fileSystem, _validator);

            //Act
            Func<Task> func = async () => await _creator.CreateReport(model);

            //Assert
            await func.Should().ThrowAsync<QCodeValidationException>()
                .WithMessage("Validation exception")
                .Where(i => i.Errors!.ContainsKey("FileName"));
        }

        [Fact]
        public async Task CreateReport_ShouldThrowQCodeValidationException_WhenNullHeaderProvided()
        {
            //Arrange
            var model = _fixture.Create<ReportRequest>();
            model.Header = null;

            _fileSystem.Directory.Exists(Arg.Any<string>()).Returns(true);
            _fileSystem.Path.HasExtension(Arg.Any<string>()).Returns(false);

            _validator = new ReportRequestValidator(_fileSystem);

            _creator = new CSVReportCreator(_logger, _fileSystem, _validator);

            //Act
            Func<Task> func = async () => await _creator.CreateReport(model);

            //Assert
            await func.Should().ThrowAsync<QCodeValidationException>()
                .WithMessage("Validation exception")
                .Where(i => i.Errors!.ContainsKey("Header"));
        }

        [Fact]
        public async Task CreateReport_ShouldThrowQCodeValidationException_WhenEmptyBodyRowsProvided()
        {
            //Arrange
            var model = _fixture.Create<ReportRequest>();
            model.Body = null;

            _fileSystem.Directory.Exists(Arg.Any<string>()).Returns(true);
            _fileSystem.Path.HasExtension(Arg.Any<string>()).Returns(false);

            _validator = new ReportRequestValidator(_fileSystem);

            _creator = new CSVReportCreator(_logger, _fileSystem, _validator);

            //Act
            Func<Task> func = async () => await _creator.CreateReport(model);

            //Assert
            await func.Should().ThrowAsync<QCodeValidationException>()
                .WithMessage("Validation exception")
                .Where(i => i.Errors!.ContainsKey("Body"));
        }
    }
}
