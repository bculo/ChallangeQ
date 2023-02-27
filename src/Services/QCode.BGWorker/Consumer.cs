using MediatR;
using QCode.Application.Common.Enums;
using QCode.Application.Features.Trades;
using QCode.Application.Interfaces;

namespace QCode.BGWorker
{
    public class Consumer : BackgroundService
    {
        private static readonly int EmptyQueueDelay = 1000;
        private static readonly int ErrorDelay = 5000;

        private readonly ILogger<Consumer> _logger;
        private readonly IServiceProvider _provider;

        public Consumer(ILogger<Consumer> logger, 
            IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consumer -> StartAsync method called");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await CheckEvents(cancellationToken);
            }
        }

        public async Task CheckEvents(CancellationToken cancellationToken)
        {
            using var scope = _provider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var storage = scope.ServiceProvider.GetRequiredService<IExtractAttemptManager>();

            var @event = storage.GetEvent();

            if(@event is null)
            {
                await Task.Delay(EmptyQueueDelay, cancellationToken);
                return;
            }

            try
            {
                _logger.LogTrace("Consumer -> Sending BuildPositionsReport.Command");

                await mediator.Send(new BuildPositionsReport.Command
                {
                    Id = Guid.NewGuid(),
                    DateTime = @event.DateTime,
                    StartOfTheDay = @event.StartOfTheDay,
                    EndOfTheDay = @event.EndOfTheDay,
                    Type = (FileType)@event.FileType,
                });

                _logger.LogTrace("Consumer -> BuildPositionsReport.Command executed");
            }
            catch
            {
                _logger.LogTrace("Consumer -> BuildPositionsReportmoz.Command failed");
                await Task.Delay(ErrorDelay, cancellationToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Consumer -> StopAsync method called");
            return base.StopAsync(cancellationToken);
        }
    }
}
