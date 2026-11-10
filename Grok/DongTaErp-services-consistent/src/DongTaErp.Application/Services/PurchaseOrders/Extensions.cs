namespace DongTaErp.Application.Services.PurchaseOrders;

internal static class Extensions
{
    public static IQueryable<PurchaseOrder> ApplySorting(this IQueryable<PurchaseOrder> query, string? sortBy = null, bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderByDescending(x => x.OrderDate);

        return sortBy.ToLower() switch
        {
            "number" => isDescending ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number),
            "orderdate" => isDescending ? query.OrderByDescending(x => x.OrderDate) : query.OrderBy(x => x.OrderDate),
            "total" => isDescending ? query.OrderByDescending(x => x.Total) : query.OrderBy(x => x.Total),
            "status" => isDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            _ => throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }

    public static IQueryable<PurchaseOrderDto> ToPurchaseOrderDto(this IQueryable<PurchaseOrder> query) =>
        query.Select(ExpressionToDto());

    public static Expression<Func<PurchaseOrder, PurchaseOrderDto>> ExpressionToDto() => x => new PurchaseOrderDto
    {
        Id = x.Id,
        Number = x.Number,
        PartnerId = x.PartnerId,
        PartnerName = x.Partner.Name,
        WarehouseId = x.WarehouseId,
        WarehouseName = x.Warehouse.Name,
        OrderDate = x.OrderDate,
        Status = x.Status,
        ConfirmedAt = x.ConfirmedAt,
        Currency = x.Currency,
        SubTotal = x.SubTotal,
        TaxRate = x.TaxRate,
        TaxAmount = x.TaxAmount,
        Total = x.Total,
        Notes = x.Notes,
        CreatedAt = x.Created,
        Lines = x.Lines.Select(l => new PurchaseOrderLineDto
        {
            Id = l.Id,
            PurchaseOrderId = l.PurchaseOrderId,
            ProductId = l.ProductId,
            ProductSku = l.Product.Sku,
            ProductName = l.Product.Name,
            Qty = l.Qty,
            UnitPrice = l.UnitPrice,
            LineTotal = l.LineTotal,
            CreatedAt = l.Created
        }).ToList()
    };
}
