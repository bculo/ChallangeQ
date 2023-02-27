using MediatR;
using Microsoft.Extensions.Options;
using QCode.Application.Common.Options;
using QCode.Application.Features.Trades;
using QCode.Application.Interfaces;
using System.Diagnostics;

namespace QCode.BGWorker
{
    public class Producer : BackgroundService
    {
        private static readonly int DefaultRunPeriod = 3;

        private readonly ILogger<Producer> _logger;
        private readonly IServiceProvider _provider;

        public Producer(ILogger<Producer> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Producer -> StartAsync method called");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteCommand(cancellationToken);
            }
        }

        public async Task ExecuteCommand(CancellationToken cancellationToken)
        {
            var stopWatch = Stopwatch.StartNew();
            using var scope = _provider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var timeService = scope.ServiceProvider.GetRequiredService<IDateTime>();
            var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<BGWorkerOptions>>();

            try
            {
                var timeInfo = timeService.DayInfo;

                _logger.LogTrace("Producer -> Sending StartReportBuildProcess.Command");

                await mediator.Send(new StartReportBuildProcess.Command
                {
                    Id = Guid.NewGuid(),
                    DateTime = timeInfo.Current,
                    StartOfTheDay = timeInfo.StartOfTheDay,
                    EndOfTheDay = timeInfo.EndOfTheDay,
                    Type = options.Value.FileType,
                });

                _logger.LogTrace("Producer -> StartReportBuildProcess.Command executed");

                await Task.Delay(GetDelayTime(options.Value.IntervalTimeInMinutes, stopWatch), cancellationToken);
            }
            catch
            {
                _logger.LogTrace("Producer -> StartReportBuildProcess.Command failed");
                await Task.Delay(GetDelayTime(options.Value.IntervalTimeInMinutes, stopWatch), cancellationToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Producer -> StopAsync method called");
            return base.StopAsync(cancellationToken);
        }

        private int GetDelayTime(int intervalInMinutes, Stopwatch watch)
        {
            watch.Stop();
            int elapsedTime = (int)watch.Elapsed.TotalMilliseconds;
            intervalInMinutes = (intervalInMinutes <= 0) ? DefaultRunPeriod : intervalInMinutes;
            var nextExecution = (intervalInMinutes * 60000) - elapsedTime;
            _logger.LogTrace("Producer -> Next execution in {0} ms", nextExecution);
            return nextExecution;
        }
    }
}