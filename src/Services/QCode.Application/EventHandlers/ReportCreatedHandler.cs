using MediatR;
using Microsoft.Extensions.Logging;
using QCode.Application.Common.Events;

namespace QCode.Application.EventHandlers
{
    public class ReportCreatedHandler : INotificationHandler<ReportCreated>
    {
        private readonly ILogger<ReportCreatedHandler> _logger;

        public ReportCreatedHandler(ILogger<ReportCreatedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(ReportCreated notification, CancellationToken cancellationToken)
        {
            //Publish via SignalR

            _logger.LogTrace("Report created. Path {0}", notification.FullPath);
            return Task.CompletedTask;
        }
    }
}
