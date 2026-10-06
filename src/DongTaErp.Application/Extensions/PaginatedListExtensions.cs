namespace DongTaErp.Application.Extensions;

public static class PaginatedListExtensions
{
    public static async Task<PaginatedList<T>> CreateAsync<T>(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var count = await source.CountAsync(ct);
        var items = await source.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync(ct);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }

    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable,
                                                                                     int pageNumber,
                                                                                     int pageSize,
                                                                                     CancellationToken ct = default) where TDestination : class
        => CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize, ct);
}