using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
	public class Plant
	{
		[Key]
		public Guid Id { get; set; }

		[StringLength(256)]
		[Required(AllowEmptyStrings = false)]
		public required string GivenName { get; set; }

		[StringLength(256)]
		[Required(AllowEmptyStrings = false)]
		public required string ScientificName { get; set; }

		public required TimeSpan SummerWateringInterval { get; set; }

		public required TimeSpan WinterWateringInterval { get; set; }

		public DateTime? LastWateredDate { get; set; }
	}
}
