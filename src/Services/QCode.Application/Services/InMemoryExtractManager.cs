using Events;
using Microsoft.Extensions.Logging;
using QCode.Application.Interfaces;
using System.Collections.Concurrent;

namespace QCode.Application.Services
{
    public class InMemoryExtractManager : IExtractAttemptManager
    {
        private readonly ILogger<InMemoryExtractManager> _logger;
        private readonly ConcurrentQueue<CreatePositionsReport> _queue;
        private readonly ConcurrentQueue<PositionsReportBuildFailed> _errorQueue;

        public InMemoryExtractManager(ILogger<InMemoryExtractManager> logger)
        {
            _logger = logger;
            _queue = new ConcurrentQueue<CreatePositionsReport>();
            _errorQueue = new ConcurrentQueue<PositionsReportBuildFailed>();
        }

        public void HandleEvent(CreatePositionsReport @event)
        {
            _queue.Enqueue(@event);
            _logger.LogTrace("Adding item with ID {0} and time {1} to queue", @event.Id, @event.DateTime);
            _logger.LogTrace("Item count {0} in queue", _queue.Count);
        }

        public CreatePositionsReport? GetEvent()
        {
            if(!_queue.TryDequeue(out CreatePositionsReport? item))
            {
                return default;
            }

            _logger.LogTrace("Item with ID {0} and time {1} removed from queue", item.Id, item.DateTime);
            _logger.LogTrace("Item count {0} in queue", _queue.Count);
            return item;
        }

        public void HandleFailedAttemptEvent(PositionsReportBuildFailed @event)
        {
            _errorQueue.Enqueue(@event);
            _logger.LogTrace("Adding item with ID {0} and time {1} to poision queue", @event.Id, @event.DateTime);
            _logger.LogTrace("Item count {0} in poison queue", _errorQueue.Count);
        }

        public PositionsReportBuildFailed? GetFailedAttemptEvent()
        {
            if (!_errorQueue.TryDequeue(out PositionsReportBuildFailed? item))
            {
                return default;
            }

            _logger.LogTrace("Item with ID {0} and time {1} removed from poision queue", item.Id, item.DateTime);
            _logger.LogTrace("Item count {0} in poison queue", _errorQueue.Count);
            return item;
        }
    }
}
