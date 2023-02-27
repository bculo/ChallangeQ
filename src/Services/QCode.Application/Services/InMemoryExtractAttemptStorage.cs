using Microsoft.Extensions.Logging;
using QCode.Application.Features.Trades;
using QCode.Application.Interfaces;
using System.Collections.Concurrent;

namespace QCode.Application.Services
{
    public class InMemoryExtractAttemptStorage : IExtractAttemptStorage
    {
        private readonly ILogger<InMemoryExtractAttemptStorage> _logger;
        private readonly ConcurrentDictionary<Guid, CreatePositionsReport.Command> _attemps;

        public InMemoryExtractAttemptStorage(ILogger<InMemoryExtractAttemptStorage> logger)
        {
            _logger = logger;
            _attemps = new ConcurrentDictionary<Guid, CreatePositionsReport.Command>();
        }

        public void AddAttempt(CreatePositionsReport.Command command)
        {
            _attemps.TryAdd(command.Id, command);
            _logger.LogTrace("Command with ID {0}, and extract time {1} added to storage", command.Id, command.DateTime);
        }

        public List<CreatePositionsReport.Command> GetFailedAttempts(int maxNumberOfAttempts)
        {
            return _attemps.Take(maxNumberOfAttempts).Select(i => i.Value).ToList();
        }

        public void RemoveAttempt(CreatePositionsReport.Command command)
        {
            if(!_attemps.ContainsKey(command.Id))
            {
                return;
            }

            _attemps.Remove(command.Id, out var fetchedCommand);
            _logger.LogTrace("Command with ID {0}, and extract time {1} removed from storage", command.Id, command.DateTime);
        }
    }
}
