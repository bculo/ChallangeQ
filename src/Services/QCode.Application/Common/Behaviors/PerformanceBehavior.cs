using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QCode.Application.Common.Options;
using System.Diagnostics;

namespace QCode.Application.Common.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
        where TRequest : notnull
    {
        private readonly ILogger<TRequest> _logger;
        private readonly PerformanceBehaviorOptions _options;

        public PerformanceBehavior(ILogger<TRequest> logger, IOptionsSnapshot<PerformanceBehaviorOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var stopWatch = Stopwatch.StartNew();

            var response = await next();

            stopWatch.Stop();

            var elapsedMilliseconds = stopWatch.ElapsedMilliseconds;
            if (elapsedMilliseconds > _options.LogPointInMiliseconds)
            {
                _logger.LogWarning("Long Running Request. Total time in miliseconds {0}", elapsedMilliseconds);
            }

            return response;
        }
    }
}
