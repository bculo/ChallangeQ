using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QCode.Core.Exceptions;

namespace QCode.RestAPI.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message);

            if (context.Exception is QCodeBaseException)
            {
                HandleCustomException(context);
                return;
            }

            var errorResponse = new
            {
                Message = "Unexpected exception",
                StatusCode = StatusCodes.Status400BadRequest
            };

            context.Result = new BadRequestObjectResult(errorResponse);
            context.ExceptionHandled = true;
        }

        private void HandleCustomException(ExceptionContext context)
        {
            var customException = context.Exception as QCodeBaseException;

            if (customException is QCodeNotFoundException)
            {
                context.Result = new NotFoundObjectResult(customException!.UserMessage);
                context.ExceptionHandled = true;
                return;
            }

            if (customException is QCodeValidationException)
            {
                var validationException = context.Exception as QCodeValidationException;

                var validatioResponse = new
                {
                    Errors = validationException!.Errors,
                    Message = validationException.UserMessage,
                    StatusCode = StatusCodes.Status400BadRequest
                };

                context.Result = new BadRequestObjectResult(validatioResponse);
                context.ExceptionHandled = true;
                return;
            }

            var errorResponse = new
            {
                Message = context.Exception.Message,
                StatusCode = StatusCodes.Status400BadRequest
            };

            context.Result = new BadRequestObjectResult(errorResponse);
            context.ExceptionHandled = true;
        }
    }
}
