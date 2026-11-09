// TEMPLATE: Copy this file to Services/[Entity]/Extensions.cs

using System.Linq.Expressions;

namespace DongTaErp.Application.Services.[Entities];

/// <summary>
/// Entity-specific sorting and DTO projection extensions.
/// 
/// These extensions work together to:
/// 1. Apply consistent sorting to queries (via ApplySorting)
/// 2. Project [Entity] domain entities to [Entity]Dto responses (via To[Entity]Dto)
/// 3. Provide a reusable OrderBy<TKey> helper to avoid ternary duplication
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Applies sorting to a queryable [Entity] collection.
    /// </summary>
    /// <param name="query">IQueryable of [Entity]</param>
    /// <param name="sortBy">Field name to sort by. Supported values: [CUSTOMIZE: e.g., "code", "name", "phone", "email"]. 
    /// Defaults to "created" (creation date descending).</param>
    /// <param name="isDescending">If true, sort descending; else ascending</param>
    /// <returns>Ordered IQueryable of [Entity]</returns>
    /// <remarks>
    /// Defaults to sorting by Created descending (newest first).
    /// All field names are case-insensitive.
    /// </remarks>
    public static IQueryable<[Entity]> ApplySorting(
        this IQueryable<[Entity]> query,
        string? sortBy = null,
        bool isDescending = false)
    {
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            // CUSTOMIZE: Add cases for each sortable field
            // "code" => OrderBy(query, e => e.Code, isDescending),
            // "name" => OrderBy(query, e => e.Name, isDescending),
            // "phone" => OrderBy(query, e => e.Phone, isDescending),
            _ => OrderBy(query, e => e.Created, isDescending) // Default: creation date
        };
    }

    /// <summary>
    /// Helper method to eliminate ternary duplication in ApplySorting.
    /// </summary>
    /// <typeparam name="TKey">Type of the sort key (string, int, DateTime, etc.)</typeparam>
    /// <param name="query">IQueryable of [Entity]</param>
    /// <param name="key">Expression selecting the property to sort by</param>
    /// <param name="isDescending">If true, sort descending; else ascending</param>
    /// <returns>Ordered IQueryable of [Entity]</returns>
    private static IOrderedQueryable<[Entity]> OrderBy<TKey>(
        IQueryable<[Entity]> query,
        Expression<Func<[Entity], TKey>> key,
        bool isDescending)
    {
        return isDescending
            ? query.OrderByDescending(key)
            : query.OrderBy(key);
    }

    /// <summary>
    /// Projects a queryable [Entity] collection to [Entity]Dto for API responses.
    /// </summary>
    /// <param name="query">IQueryable of [Entity]</param>
    /// <returns>IQueryable of [Entity]Dto with all required fields mapped</returns>
    /// <remarks>
    /// This projection runs server-side in EF Core, ensuring only necessary columns are selected.
    /// Always call this before PaginatedListAsync or FirstOrDefaultAsync to avoid loading full entities.
    /// </remarks>
    public static IQueryable<[Entity]Dto> To[Entity]Dto(this IQueryable<[Entity]> query)
    {
        return query.Select(e => new [Entity]Dto
        {
            Id = e.Id,
            // CUSTOMIZE: Map all entity properties to DTO
            // Code = e.Code,
            // Name = e.Name,
            // Phone = e.Phone,
            CreatedAt = e.Created
        });
    }
}
