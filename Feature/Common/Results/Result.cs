using Application.Common.Results.Abstractions;
using Application.Common.Validation;

namespace Application.Common.Results
{
	public sealed class Result : IResult<Result>
	{
		public bool IsSuccess { get; }
		public Error? Error { get; }
		public IReadOnlyDictionary<string, string[]> ValidationErrors { get; }

		private Result()
		{
			IsSuccess = true;
			ValidationErrors = new Dictionary<string, string[]>();
		}

		private Result(Error error, Dictionary<string, string[]> validationErrors)
		{
			IsSuccess = false;
			Error = error;
			ValidationErrors = validationErrors;
		}

		public static Result Success() => new();

		public static Result Failure(Error error) => new(error, new());

		public static Result ValidationFailure(Error error, ValidationScope validationScope) => new(error, validationScope.ToDictionary());
	}

	public sealed class Result<T> : IResult<Result<T>>
	{
		public bool IsSuccess { get; }
		public T? Value { get; }
		public Error? Error { get; }
		public IReadOnlyDictionary<string, string[]> ValidationErrors { get; }

		private Result(T value)
		{
			IsSuccess = true;
			Value = value;
			ValidationErrors = new Dictionary<string, string[]>();
		}

		private Result(Error error, Dictionary<string, string[]> validationErrors)
		{
			IsSuccess = false;
			Error = error;
			ValidationErrors = validationErrors;
		}

		public static Result<T> Success(T value) => new(value);

		public static Result<T> Failure(Error error) => new(error, new());

		public static Result<T> ValidationFailure(Error error, ValidationScope validationScope) => new(error, validationScope.ToDictionary());
	}
}
