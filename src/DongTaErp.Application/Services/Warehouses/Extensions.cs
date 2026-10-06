namespace DongTaErp.Application.Services.Warehouses;

internal static class Extensions
{
    public static IQueryable<Warehouse> ApplySorting(this IQueryable<Warehouse> query, string? sortBy=null, bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return query.OrderByDescending(w => w.Created);
        }
        return sortBy.ToLower() switch
        {
            "code" => isDescending ? query.OrderByDescending(w => w.Code) : query.OrderBy(w => w.Code),
            "name" => isDescending ? query.OrderByDescending(w => w.Name) : query.OrderBy(w => w.Name),
            "countrycode" => isDescending ? query.OrderByDescending(w => w.CountryCode) : query.OrderBy(w => w.CountryCode),
            "city" => isDescending ? query.OrderByDescending(w => w.City) : query.OrderBy(w => w.City),
            "address" => isDescending ? query.OrderByDescending(w => w.Address) : query.OrderBy(w => w.Address),
            _ => throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }
    public static IQueryable<WarehouseDto> ToWarehouseDto(this IQueryable<Warehouse> query) => query.Select(ExpressionToDto());
    public static Expression<Func<Warehouse, WarehouseDto>> ExpressionToDto() => w => new WarehouseDto
    {
        Id = w.Id,
        Code = w.Code,
        Name = w.Name,
        Type = w.Type,
        CountryCode = w.CountryCode,
        City = w.City,
        Address = w.Address,
        TimeZone = w.TimeZone,
        IsDefault = w.IsDefault,
        IsActive = w.IsActive,
        CreatedAt = w.Created
    };
}
