using Application.Common.Results;

namespace Application.Common.Errors
{
	public static class UserErrors
	{
		public static Error NotFound => new(ErrorType.NotFound, "Users.NotFound", "Provided user was not found");

		public static Error NotUnique => new(ErrorType.Validation, "Users.NotUnique", "User with provided email already exists");

		public static Error InvalidToken => new(ErrorType.Validation, "Users.InvalidToken", "Operation failed, token was invalid");

		public static Error InvalidPassword => new(ErrorType.Validation, "Users.InvalidPassword", "Password does not meet the security requirements");
	}
}
