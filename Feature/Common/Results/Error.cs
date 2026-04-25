namespace Application.Common.Results
{
	public sealed record Error
	{
		public ErrorType Type { get; init; }

		public string Code { get; init; } = string.Empty;
		 
		public string Message { get; init; } = string.Empty;

		public Error(ErrorType type, string code, string message)
		{
			Type = type;
			Code = code;
			Message = message;
		}
	}
}
