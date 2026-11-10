namespace DongTaErp.Application.Services.GoodsReceipts;

internal static class Extensions
{
    public static IQueryable<GoodsReceipt> ApplySorting(this IQueryable<GoodsReceipt> query, string? sortBy = null, bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderByDescending(x => x.ReceiptDate);

        return sortBy.ToLower() switch
        {
            "code" => isDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
            "receiptdate" => isDescending ? query.OrderByDescending(x => x.ReceiptDate) : query.OrderBy(x => x.ReceiptDate),
            "status" => isDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            _ => throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }

    public static IQueryable<GoodsReceiptDto> ToGoodsReceiptDto(this IQueryable<GoodsReceipt> query) =>
        query.Select(ExpressionToDto());

    public static Expression<Func<GoodsReceipt, GoodsReceiptDto>> ExpressionToDto() => x => new GoodsReceiptDto
    {
        Id = x.Id,
        Code = x.Code,
        PurchaseOrderId = x.PurchaseOrderId,
        PurchaseOrderNumber = x.PurchaseOrder.Number,
        WarehouseId = x.WarehouseId,
        WarehouseName = x.Warehouse.Name,
        ReceiptDate = x.ReceiptDate,
        Status = x.Status,
        PostedAt = x.PostedAt,
        CancelledAt = x.CancelledAt,
        SupplierRef = x.SupplierRef,
        Notes = x.Notes,
        CreatedAt = x.Created,
        Lines = x.Lines.Select(l => new GoodsReceiptLineDto
        {
            Id = l.Id,
            GoodsReceiptId = l.GoodsReceiptId,
            PurchaseOrderLineId = l.PurchaseOrderLineId,
            ProductId = l.ProductId,
            ProductSku = l.Product.Sku,
            ProductName = l.Product.Name,
            Quantity = l.Quantity,
            Note = l.Note,
            CreatedAt = l.Created
        }).ToList()
    };
}
