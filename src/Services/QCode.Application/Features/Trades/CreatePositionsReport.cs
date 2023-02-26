﻿using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using QCode.Application.Common.Enums;
using QCode.Application.Common.Models;
using QCode.Application.Common.Options;
using QCode.Application.Interfaces;
using QCode.Application.Services;
using Services;

namespace QCode.Application.Features.Trades
{
    public static class CreatePositionsReport
    {
        public class Command : IRequest 
        {
            public DateTime DateTime { get; set; }
            public DateTime StartOfTheDay { get; set; }
            public DateTime EndOfTheDay { get; set; }
            public FileType Type { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IPowerService _powerService;
            private readonly IReportCreatorFactory _factory;
            private FileReportOptions _options;

            public Handler(ILogger<Handler> logger,
                IPowerService powerService,
                IReportCreatorFactory factory,
                IOptionsSnapshot<FileReportOptions> options)
            {
                _logger = logger;
                _powerService = powerService;
                _factory = factory;
                _options = options.Value;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var polly = Policy.Handle<Exception>()
                    .WaitAndRetryAsync(3,
                        _ => TimeSpan.FromSeconds(1),
                        (Exception e, TimeSpan s) => _logger.LogError(e, e.Message));

                var trades = await polly.ExecuteAsync(async () => await _powerService.GetTradesAsync(request.DateTime));

                var reportCreator = _factory.CreateFileCreator(request.Type);

                var reportRequest = CreateReportRequestModel(trades, request);

                await reportCreator.CreateReport(reportRequest);
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