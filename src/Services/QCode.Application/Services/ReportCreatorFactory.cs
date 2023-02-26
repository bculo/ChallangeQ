using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QCode.Application.Common.Enums;
using QCode.Application.Interfaces;
using QCode.Core.Exceptions;

namespace QCode.Application.Services
{
    public class ReportCreatorFactory : IReportCreatorFactory
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<ReportCreatorFactory> _logger;

        public ReportCreatorFactory(IServiceProvider provider,
            ILogger<ReportCreatorFactory> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public IReportCreator CreateFileCreator(FileType type)
        {
            _logger.LogTrace("Creating file generator for type {0}", type);

            switch (type)
            {
                case FileType.CSV:
                    return _provider.GetRequiredService<CSVReportCreator>();
                case FileType.TXT:
                    return _provider.GetRequiredService<TxtReportCreator>();
                default:
                    throw new QCodeBaseException($"File generator for type {type} not found");
            }
        }
    }
}
