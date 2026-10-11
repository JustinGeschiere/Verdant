using Application.Common.Errors;
using Application.Common.Results;
using Application.Common.Results.Abstractions;
using FluentValidation;
using Mediator;

namespace Application.Common.Behaviors
{
	public sealed class ValidationBehavior<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
		where TMessage : IMessage
		where TResponse : IResult<TResponse>
	{
		private readonly IEnumerable<IValidator<TMessage>> _validators;

		public ValidationBehavior(IEnumerable<IValidator<TMessage>> validators)
		{
			_validators = validators;
		}

		public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
		{
			if (!_validators.Any())
			{
				return await next(message, cancellationToken);
			}

			var context = new ValidationContext<TMessage>(message);

			var validationResults = await Task.WhenAll(_validators.Select(i => i.ValidateAsync(context, cancellationToken)));

			var failures = validationResults
				.SelectMany(i => i.Errors)
				.ToList();

			if (failures.Count == 0)
			{
				return await next(message, cancellationToken);
			}

			var failureScope = FailureScope.Run(scope =>
			{
				foreach (var failure in failures)
				{
					scope.FieldError(failure.PropertyName, failure.ErrorMessage);
				}
			});

			return TResponse.ValidationFailure(CommonErrors.Validation, failureScope);
		}
	}
}
