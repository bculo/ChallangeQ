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
                    return GetInstance<CSVReportCreator>();
                case FileType.TXT:
                    return GetInstance<TxtReportCreator>();
                default:
                    throw new QCodeBaseException($"File generator for type {type} not found");
            }
        }

        private IReportCreator GetInstance<T>() where T : BaseReportCreator
        {
            T? instance = _provider.GetService<T>() as T;
            ArgumentNullException.ThrowIfNull(instance, nameof(instance));
            return instance;
        }
    }
}
