namespace Contracts.Models.Shared.Responses
{
	public sealed record PagedResponse<T>
	{
		public int PageIndex { get; init; } = 0;
		public int PageSize { get; init; } = 10;
		public int TotalCount { get; init; } = 0;
		public List<T> Items { get; init; } = [];

		public PagedResponse(int pageIndex, int pageSize, List<T> items)
		{
			PageIndex = pageIndex;
			PageSize = pageSize;

			if (items != null)
			{
				Items = items;
				TotalCount = items.Count;
			}
		}

		public PagedResponse(int pageIndex, int pageSize, IEnumerable<T> items)
			: this(pageIndex, pageSize, items?.ToList() ?? [])
		{ }
	}
}
