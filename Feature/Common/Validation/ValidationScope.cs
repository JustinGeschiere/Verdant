namespace Application.Common.Validation
{
	public sealed class ValidationScope
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

		public static ValidationScope Run(Action<ValidationScope> action)
		{
			var scope = new ValidationScope();
			action(scope);

			return scope;
		}

		public static async Task<ValidationScope> Run(Func<ValidationScope, Task> action)
		{
			var scope = new ValidationScope();
			await action(scope);

			return scope;
		}
	}
}
