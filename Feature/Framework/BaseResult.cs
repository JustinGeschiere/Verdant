namespace Feature.Framework
{
	public record BaseResult<T>(T status) where T : Enum
	{
		public T Status { get; } = status;
	}

	public record BaseResult(bool success)
	{
		public bool Success { get; } = success;
	}
}
