namespace DongTaErp.Application.Common.Models;

public record PaginatedList<T>(IReadOnlyList<T> Items, int TotalCount, int PageNumber = 1, int PageSize = 20)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}