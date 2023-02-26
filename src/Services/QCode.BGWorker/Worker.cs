using MediatR;
using Microsoft.Extensions.Options;
using QCode.Application.Common.Options;
using QCode.Application.Features.Trades;
using QCode.Application.Interfaces;
using QCode.Core.Exceptions;
using System.Diagnostics;

namespace QCode.BGWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _provider;

        public Worker(ILogger<Worker> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync method called in Worker service");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var stopWatch = Stopwatch.StartNew();
                using var scope = _provider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var timeService = scope.ServiceProvider.GetRequiredService<IDateTime>();
                var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<BGWorkerOptions>>();

                _logger.LogTrace("Scope created inside Worker");

                try
                {
                    var timeInfo = timeService.DayInfo;

                    _logger.LogTrace("Sending CreatePositionsReport.Command");

                    await mediator.Send(new CreatePositionsReport.Command
                    {
                        DateTime = timeInfo.Current,
                        StartOfTheDay = timeInfo.StartOfTheDay,
                        EndOfTheDay = timeInfo.EndOfTheDay,
                        Type = options.Value.FileType,
                    });

                    _logger.LogTrace("CreatePositionsReport.Command executed");

                    await Task.Delay(GetDelayTime(options.Value.IntervalTimeInMinutes, stopWatch), stoppingToken);
                }
                catch(Exception e)
                {
                    if (e is QCodeCriticalException)
                    {
                        _logger.LogCritical("Shutting down background service... Reason: {0}", e.Message);
                        break;
                    }

                    _logger.LogTrace("CreatePositionsReport.Command failed");
                    await Task.Delay(GetDelayTime(options.Value.IntervalTimeInMinutes, stopWatch), stoppingToken);
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("StopAsync method called in Worker service");
            return base.StopAsync(cancellationToken);
        }

        private int GetDelayTime(int intervalInMinutes, Stopwatch watch)
        {
            watch.Stop();

            int elapsedTime = (int)watch.Elapsed.TotalMilliseconds;

            if (intervalInMinutes <= 0)
            {
                return (3 * 60000) - elapsedTime;
            }

            var nextExecution = (intervalInMinutes * 60000) - elapsedTime;

            _logger.LogTrace("Next execution in {0} ms", nextExecution);

            return nextExecution;
        }
    }
}