namespace DongTaErp.Application.Services.Products;

internal static class Extensions
{
    public static IQueryable<Product> ApplySorting(this IQueryable<Product> query, string? sortBy = null, bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return query.OrderByDescending(p => p.Created);
        }
        return sortBy.ToLower() switch
        {
            "sku" => isDescending ? query.OrderByDescending(p => p.Sku) : query.OrderBy(p => p.Sku),
            "name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "unit" => isDescending ? query.OrderByDescending(p => p.Unit) : query.OrderBy(p => p.Unit),
            "costprice" => isDescending ? query.OrderByDescending(p => p.CostPrice) : query.OrderBy(p => p.CostPrice),
            "saleprice" => isDescending ? query.OrderByDescending(p => p.SalePrice) : query.OrderBy(p => p.SalePrice),
            _ => throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }
    public static IQueryable<ProductDto> ToProductDto(this IQueryable<Product> query)
    {
        return query.Select(ExpressionToDto());
    }

    public static Expression<Func<Product, ProductDto>> ExpressionToDto()
    {
        return p => new ProductDto
        {
            Id = p.Id,
            Sku = p.Sku,
            Name = p.Name,
            CategoryName = p.Category != null
                ? p.Category.Name
                : null,
            Unit = p.Unit,
            CostPrice = p.CostPrice,
            SalePrice = p.SalePrice,
            MinStock = p.MinStock,
            IsActive = p.IsActive,
            CreatedAt = p.Created
        };
    }
}
