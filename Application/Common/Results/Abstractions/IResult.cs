using Application.Common.Validation;

namespace Application.Common.Results.Abstractions
{
	public interface IResult<TSelf>
		where TSelf : IResult<TSelf>
	{
		bool IsSuccess { get; }
		Error? Error { get; }
		IReadOnlyDictionary<string, string[]> ValidationErrors { get; }

		static abstract TSelf Failure(Error error);

		static abstract TSelf ValidationFailure(Error error, ValidationScope validationScope);
	}
}
