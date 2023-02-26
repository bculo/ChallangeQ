using FluentValidation;
using MediatR;
using QCode.Core.Exceptions;

namespace QCode.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var validationTasks = _validators.Select(v => v.ValidateAsync(context, cancellationToken));

            var validationResults = await Task.WhenAll(validationTasks);

            var failures = validationResults
                  .SelectMany(result => result.Errors)
                  .Where(f => f != null)
                  .ToList();

            if (failures.Count != 0)
            {
                var errors = failures
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(x => x.Key, y => y.Select(i => i.ErrorMessage).ToArray());

                throw new QCodeValidationException(errors);
            }

            return await next();
        }
    }
}
