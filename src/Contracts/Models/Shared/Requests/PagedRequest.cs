namespace Contracts.Models.Shared.Requests;

public sealed record PagedRequest
{
	int PageIndex { get; init; } = 0;

	int PageSize { get; init; } = 10;

	public static PagedRequest Max()
	{
		return new PagedRequest()
		{
			PageIndex = 0,
			PageSize = int.MaxValue - 1
		};
	}
}
