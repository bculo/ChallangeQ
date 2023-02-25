using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using QCode.Application.Common.Enums;
using QCode.Application.Interfaces;
using QCode.Application.Services;
using QCode.Core.Exceptions;
using Services;

namespace QCode.Application.Features.Trades
{
    public static class CreateTradePositionsReport
    {
        public class Command : IRequest 
        {
            public DateTime DateTime { get; set; }
            public DateTime StartOfTheDay { get; set; }
            public DateTime EndOfTheDay { get; set; }
            public FileType Type { get; set; }
            public string? FileLocation { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IPowerService _powerService;
            private readonly IReportCreatorFactory _factory;

            public Handler(ILogger<Handler> logger,
                IPowerService powerService,
                IReportCreatorFactory factory)
            {
                _logger = logger;
                _powerService = powerService;
                _factory = factory;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var trades = await _powerService.GetTradesAsync(request.DateTime);

                if(trades is null || !trades.Any())
                {
                    throw new QCodeBaseException("Powerservice returned empty array or null");
                }

                var reportCreator = _factory.CreateFileCreator(request.Type);

                var reportRequest = CreateReportRequestModel(trades, request);

                await reportCreator.CreateReport(reportRequest);
            }

            private ReportRequest CreateReportRequestModel(IEnumerable<PowerTrade> trades, Command request)
            {
                var slidingDate = request.StartOfTheDay;
                var periodCalcs = CalculateVolumeByPeriod(trades);

                var reportReq = new ReportRequest
                {
                    FileName = request.DateTime.ToString("YYYY-DD"),
                    SaveToLocation = request.FileLocation,
                    Header = new ReportRow(),
                    Body = new List<ReportRow>()
                };

                reportReq.Header.Items = new List<ReportRowItem>
                {
                    new ReportRowItem { Text = "Local Time", Width = 10, Alignment = ReportRowItemAlignment.LEFT },
                    new ReportRowItem { Text = "Volume", Width = 10, Alignment = ReportRowItemAlignment.LEFT }
                };

                foreach (var item in periodCalcs)
                {
                    reportReq.Body.Add(new ReportRow
                    {
                        Items = new List<ReportRowItem>
                        {
                            new ReportRowItem { Text = slidingDate.ToString("HH:mm"), Width = 10, Alignment = ReportRowItemAlignment.RIGHT },
                            new ReportRowItem { Text = ((int)item.Value).ToString(), Width = 10, Alignment = ReportRowItemAlignment.RIGHT }
                        }
                    });

                    slidingDate = slidingDate.AddHours(1);
                }

                return reportReq;
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
