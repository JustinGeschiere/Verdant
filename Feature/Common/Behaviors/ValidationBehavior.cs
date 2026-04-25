using Application.Common.Errors;
using Application.Common.Results.Abstraction;
using Application.Common.Validation;
using FluentValidation;
using MediatR;

namespace Application.Common.Behaviors
{
	public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : notnull
		where TResponse : IResult<TResponse>
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

			var validationResults = await Task.WhenAll(_validators.Select(i => i.ValidateAsync(context, cancellationToken)));

			var failures = validationResults
				.SelectMany(i => i.Errors)
				.ToList();

			if (failures.Count == 0)
			{
				return await next();
			}

			var validationScope = ValidationScope.Run(scope =>
			{
				foreach (var failure in failures)
				{
					scope.FieldError(failure.PropertyName, failure.ErrorMessage);
				}
			});

			return TResponse.ValidationFailure(CommonErrors.Validation, validationScope);
		}
	}
}
