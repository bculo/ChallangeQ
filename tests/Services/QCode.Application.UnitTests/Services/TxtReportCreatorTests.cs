using Microsoft.Extensions.Logging;
using NSubstitute;
using QCode.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCode.Application.UnitTests.Services
{
    public class TxtReportCreatorTests : BaseReportCreatorTests
    {
        private readonly ILogger<TxtReportCreator> _logger = Substitute.For<ILogger<TxtReportCreator>>();

        public TxtReportCreatorTests() : base() { }

        public override BaseReportCreator CreateInstance()
        {
            return new TxtReportCreator(_logger, _fileSystem, _validator);
        }

        public override string GetExtension()
        {
            return ".txt";
        }
    }
}
