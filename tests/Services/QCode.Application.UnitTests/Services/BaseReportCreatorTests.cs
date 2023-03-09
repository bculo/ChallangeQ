using AutoFixture;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using QCode.Application.Common.Models;
using QCode.Application.Interfaces;
using QCode.Application.Services;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCode.Application.UnitTests.Services
{
    public class BaseReportCreatorTests
    {
        protected readonly Fixture _fixture = new Fixture();

        protected IReportCreator? _creator;
        protected IFileSystem _fileSystem = Substitute.For<IFileSystem>();
        protected IValidator<ReportRequest> _validator = Substitute.For<IValidator<ReportRequest>>();
    }
}
