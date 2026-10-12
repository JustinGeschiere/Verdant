using System.ComponentModel;

namespace Contracts.Models.Shared.Requests;

public sealed record SortedRequest
{
	string SortProperty { get; init; } = "Id";

	ListSortDirection SortDirection { get; init; } = ListSortDirection.Ascending;
}
