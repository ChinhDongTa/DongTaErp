namespace DongTaErp.Application.Services.InvantoryTxn;

internal static class Extensions
{
    public static IQueryable<InventoryTxn> ApplySorting(this IQueryable<InventoryTxn> query, string? sortBy = null, bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return query.OrderByDescending(x => x.Created);
        }
        return sortBy.ToLower() switch
        {
            "productsku" => isDescending
                ? query.OrderByDescending(x => x.Product.Sku)
                : query.OrderBy(x => x.Product.Sku),
            "productname" => isDescending
                ? query.OrderByDescending(x => x.Product.Name)
                : query.OrderBy(x => x.Product.Name),
            "warehousename" => isDescending
                ? query.OrderByDescending(x => x.Warehouse.Name)
                : query.OrderBy(x => x.Warehouse.Name),
            "qtychange" => isDescending
                ? query.OrderByDescending(x => x.QtyChange)
                : query.OrderBy(x => x.QtyChange),
            "reference" => isDescending
                ? query.OrderByDescending(x => x.Reference)
                : query.OrderBy(x => x.Reference),
           
            _ => throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }
    public static IQueryable<InventoryTxnDto> ToInventoryTxnDto(this IQueryable<InventoryTxn> query)
    {
        return query.Select(ExpressionToInventoryTxnDto());
    }

    public static Expression<Func<InventoryTxn, InventoryTxnDto>> ExpressionToInventoryTxnDto()
    {
        return x => new InventoryTxnDto
        {
            Id = x.Id,
            ProductSku = x.Product.Sku,
            ProductName = x.Product.Name,
            WarehouseName = x.Warehouse.Name,
            ContraWarehouseName = x.ContraWarehouse != null
                ? x.ContraWarehouse.Name
                : null,
            Type = x.Type,
            QtyChange = x.QtyChange,
            Reference = x.Reference,
            Note = x.Note,
            CreatedAt = x.Created
        };
    }
}
