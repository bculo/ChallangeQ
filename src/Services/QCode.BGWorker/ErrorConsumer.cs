using MediatR;
using QCode.Application.Common.Enums;
using QCode.Application.Features.Trades;
using QCode.Application.Interfaces;

namespace QCode.BGWorker
{
    public class ErrorConsumer : BackgroundService
    {
        private static readonly int EmptyQueueDelay = 1000;
        private static readonly int ErrorDelay = 30000;

        private readonly ILogger<ErrorConsumer> _logger;
        private readonly IServiceProvider _provider;

        public ErrorConsumer(ILogger<ErrorConsumer> logger, 
            IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ErrorConsumer -> StartAsync method called");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await BuildPositionReport(cancellationToken);
            }
        }

        public async Task BuildPositionReport(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _provider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var manager = scope.ServiceProvider.GetRequiredService<IExtractAttemptManager>();

                var @event = manager.GetFailedAttemptEvent();

                if (@event is null)
                {
                    await Task.Delay(EmptyQueueDelay, cancellationToken);
                    return;
                }

                _logger.LogTrace("ErrorConsumer -> Sending BuildPositionsReport.Command");

                await mediator.Send(new BuildPositionsReport.Command
                {
                    Id = Guid.NewGuid(),
                    DateTime = @event.DateTime,
                    StartOfTheDay = @event.StartOfTheDay,
                    EndOfTheDay = @event.EndOfTheDay,
                    Type = (FileType)@event.FileType,
                });

                _logger.LogTrace("ErrorConsumer -> BuildPositionsReport.Command executed");
            }
            catch
            {
                _logger.LogTrace("ErrorConsumer -> BuildPositionsReport.Command failed");
                await Task.Delay(ErrorDelay, cancellationToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("ErrorConsumer -> StopAsync method called");
            return base.StopAsync(cancellationToken);
        }
    }
}
