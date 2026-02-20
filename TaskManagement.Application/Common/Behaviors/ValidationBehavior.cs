using FluentValidation;
using MediatR;

namespace TaskManagement.Application.Common.Behaviors;

// IPipelineBehavior<TRequest, TResponse> wraps every MediatR request
// TRequest must implement IRequest<TResponse>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        // Run all validators and collect ALL failures at once
        // instead of stopping at the first error
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        // Validation passed — proceed to the handler
        return await next();
    }
}
