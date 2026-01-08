namespace Feature.Framework
{
	public class BaseResult<T>(T status) where T : Enum
	{
		public T Status { get; } = status;
	}

	public class BaseResult(bool success)
	{
		public bool Success { get; } = success;
	}
}
