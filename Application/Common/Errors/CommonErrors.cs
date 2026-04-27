using Application.Common.Results;

namespace Application.Common.Errors
{
	public static class CommonErrors
	{
		public static Error Validation => new(ErrorType.Validation, "Common.Validation", "One or more validation errors occurred");

		public static Error Failure => new(ErrorType.Failure, "Common.Failure", "An unexpected error occurred, please try again later");
	}
}
