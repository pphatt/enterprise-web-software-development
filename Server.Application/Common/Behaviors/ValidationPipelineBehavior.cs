﻿using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Server.Application.Common.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
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

        var validationResult = await Task.WhenAll(
            _validators.Select(vr => vr.ValidateAsync(context, cancellationToken))
        );

        bool validationResultAreValid = validationResult.All(vr => vr.IsValid);

        if (validationResultAreValid)
        {
            return await next();
        }

        List<ValidationFailure> validationFailures =
            validationResult
                .SelectMany(vr => vr.Errors)
                .Where(error => error != null)
                .ToList();

        return (dynamic)validationFailures.ConvertAll(
            validationFail => Error.Validation(
                code: validationFail.PropertyName,
                description: validationFail.ErrorMessage
            )
        );
    }
}
