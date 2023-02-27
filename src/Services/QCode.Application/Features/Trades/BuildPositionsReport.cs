using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using QCode.Application.Common.Enums;
using QCode.Application.Common.Events;
using QCode.Application.Common.Models;
using QCode.Application.Common.Options;
using QCode.Application.Interfaces;
using QCode.Application.Services;
using Services;

namespace QCode.Application.Features.Trades
{
    public static class BuildPositionsReport
    {
        public class Command : IRequest 
        {
            public Guid Id { get; set; }
            public DateTime DateTime { get; set; }
            public DateTime StartOfTheDay { get; set; }
            public DateTime EndOfTheDay { get; set; }
            public FileType Type { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IMediator _mediator;
            private readonly ILogger<Handler> _logger;
            private readonly IPowerService _powerService;
            private readonly IReportCreatorFactory _factory;
            private readonly IExtractAttemptManager _manager;
            private FileReportOptions _options;
            private PowerServiceOptions _psOptions;

            public Handler(ILogger<Handler> logger,
                IPowerService powerService,
                IReportCreatorFactory factory,
                IOptionsSnapshot<FileReportOptions> options,
                IOptions<PowerServiceOptions> psOptions,
                IMediator mediator,
                IExtractAttemptManager manager)
            {
                _logger = logger;
                _powerService = powerService;
                _factory = factory;
                _options = options.Value;
                _psOptions = psOptions.Value;
                _mediator = mediator;
                _manager = manager;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    _logger.LogTrace("Fetching trades from {0}", nameof(IPowerService));

                    var policy = GetExecutionPolicy();
                    var trades = await policy.ExecuteAsync(async () => await _powerService.GetTradesAsync(request.DateTime));

                    _logger.LogTrace("Trades fetched from {0}. Creating positions report...", nameof(IPowerService));

                    var reportCreator = _factory.CreateFileCreator(request.Type);
                    var reportRequest = CreateReportRequestModel(trades, request);
                    var filePath = await reportCreator.CreateReport(reportRequest);

                    await _mediator.Publish(new ReportCreated
                    {
                        FullPath = filePath
                    });
                }
                catch
                {
                    _manager.HandleFailedAttemptEvent(new Events.PositionsReportBuildFailed
                    {
                        DateTime = request.DateTime,
                        EndOfTheDay = request.EndOfTheDay,
                        FileType = (int)request.Type,
                        Id = request.Id,
                        StartOfTheDay = request.StartOfTheDay,
                    });

                    throw;
                }
            }

            private AsyncPolicy GetExecutionPolicy()
            {
                var overallTimeoutPolicy = Policy.TimeoutAsync(_psOptions.Timeout);

                var waitAndRetryPolicy = Policy.Handle<Exception>()
                    .WaitAndRetryAsync(_psOptions.RetryCount,
                        _ => TimeSpan.FromSeconds(3),
                       (Exception e, TimeSpan s) => _logger.LogError(e, $"Retrying after error -> {e.Message}"));

                return overallTimeoutPolicy.WrapAsync(waitAndRetryPolicy);
            }

            private ReportRequest CreateReportRequestModel(IEnumerable<PowerTrade> trades, Command request)
            {
                var slidingDate = request.StartOfTheDay;
                var periodDict = CalculateVolumeByPeriod(trades);

                var fileName = $"{_options.FileNamePrefix}_{request.DateTime.ToString(_options.DateFormat!)}";
                var builder = new ReportContentBuilder(fileName, _options!.FileLocation!);

                builder.AddHeaderItem("Local Time", 10, ReportRowItemAlignment.RIGHT)
                    .AddHeaderItem("Volume", 10, ReportRowItemAlignment.RIGHT);

                foreach (var item in periodDict)
                {
                    builder.AddBodyRow()
                        .AddBodyRowItem(slidingDate.ToString("HH:mm"), 10, ReportRowItemAlignment.RIGHT)
                        .AddBodyRowItem(Math.Round(item.Value, 2).ToString(), 10, ReportRowItemAlignment.RIGHT);

                    slidingDate = slidingDate.AddHours(1);
                }

                return builder.Build();
            }

            private Dictionary<int, double> CalculateVolumeByPeriod(IEnumerable<PowerTrade> trades)
            {
                return trades.SelectMany(i => i.Periods)
                    .GroupBy(i => i.Period)
                    .OrderBy(i => i.Key)
                    .ToDictionary(key => key.Key, values => values.Select(i => i.Volume).Sum());
            }
        }
    }
}
