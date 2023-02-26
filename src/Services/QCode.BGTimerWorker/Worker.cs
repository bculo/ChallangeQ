using MediatR;
using Microsoft.Extensions.Options;
using QCode.Application.Common.Options;
using QCode.Application.Features.Trades;
using QCode.Application.Interfaces;
using QCode.Core.Exceptions;

namespace QCode.BGTimerWorker
{
    public class Worker : IHostedService, IDisposable
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _provider;
        private readonly IConfiguration _configuration;

        private Timer? _timer = null;

        public Worker(ILogger<Worker> logger, 
            IConfiguration configuration,
            IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            var runPeriod = _configuration.GetValue<int>("BGWorkerOptions:IntervalTimeInMinutes");

            _logger.LogInformation("Setting timer. Service will execute extract every {0} minute/minutes", runPeriod);

            _timer = new Timer(ExecuteExtract, null, TimeSpan.Zero, TimeSpan.FromMinutes(runPeriod));

            return Task.CompletedTask;
        }

        private void ExecuteExtract(object? state)
        {
            _logger.LogTrace("Creating scope inside ExecuteExtract method");

            using var scope = _provider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var timeService = scope.ServiceProvider.GetRequiredService<IDateTime>();
            var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<BGWorkerOptions>>();

            _logger.LogTrace("Scope created inside Worker");

            try
            {
                var timeInfo = timeService.DayInfo;

                _logger.LogTrace("Sending CreatePositionsReport.Command");

                mediator.Send(new CreatePositionsReport.Command
                {
                    DateTime = timeInfo.Current,
                    StartOfTheDay = timeInfo.StartOfTheDay,
                    EndOfTheDay = timeInfo.EndOfTheDay,
                    Type = options.Value.FileType,
                }).GetAwaiter().GetResult();

                _logger.LogTrace("CreatePositionsReport.Command executed");
            }
            catch (Exception e)
            {
                if (e is QCodeCriticalException)
                {
                    _logger.LogCritical("Shutting down background service... Reason: {0}", e.Message);
                    StopAsync(new());
                }

                _logger.LogTrace("CreatePositionsReport.Command failed");
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}