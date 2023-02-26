using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QCode.Core.Exceptions;

namespace QCode.Application.Common.Behaviors
{
    public class ExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<TRequest> _logger;

        public ExceptionBehavior(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (Exception e)
            {
                if(e is QCodeValidationException)
                {
                    LogValidationException((e as QCodeValidationException)!);
                }
                else
                {
                    LogStandardMessage(e);
                }

                throw;
            }
        }

        private void LogValidationException(QCodeValidationException exception)
        {
            if(exception.Errors is null)
            {
                LogStandardMessage(exception);
            }

            var jsonErros = JsonConvert.SerializeObject(exception.Errors);
            _logger.LogError(exception, jsonErros);
        }

        private void LogStandardMessage(Exception exception)
        {
            _logger.LogError(exception, exception.Message);
        }
    }
}
