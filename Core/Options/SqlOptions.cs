using System.ComponentModel.DataAnnotations;

namespace Core.Options;

public class SqlOptions
{
	public const string SECTION = "Sql";

	[Required(AllowEmptyStrings = false)]
	public required string ConnectionString { get; set; }
}
