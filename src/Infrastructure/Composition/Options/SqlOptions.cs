using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Composition.Options;

public class SqlOptions
{
	public const string SECTION = "Sql";

	[Required(AllowEmptyStrings = false)]
	public required string ConnectionString { get; set; }
}
