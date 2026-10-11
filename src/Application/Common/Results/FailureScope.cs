namespace Application.Common.Results
{
	public sealed class FailureScope
	{
		private readonly Dictionary<string, string[]> _errors = new();

		public void FieldError(string field, string message)
		{
			if (!_errors.ContainsKey(field))
			{
				_errors[field] = [];
			}

			_errors[field] = _errors[field].Append(message).ToArray();
		}

		public void GlobalError(string message)
		{
			FieldError(string.Empty, message);
		}

		public bool HasErrors => _errors.Count > 0;

		public Dictionary<string, string[]> ToDictionary()
			=> new(_errors);

		public static FailureScope Run(Action<FailureScope> action)
		{
			var scope = new FailureScope();
			action(scope);

			return scope;
		}

		public static async Task<FailureScope> Run(Func<FailureScope, Task> action)
		{
			var scope = new FailureScope();
			await action(scope);

			return scope;
		}
	}
}
