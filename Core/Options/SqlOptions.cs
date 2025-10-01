using System.ComponentModel.DataAnnotations;

namespace Core.Options;

public class SqlOptions
{
	const string SECTION = "Sql";

	[Required(AllowEmptyStrings = false)]
	public string ConnectionString { get; set; }
}
