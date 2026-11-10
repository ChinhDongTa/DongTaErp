namespace DongTaErp.Application.Services.VendorBills;

internal static class Extensions
{
    public static IQueryable<VendorBill> ApplySorting(this IQueryable<VendorBill> query, string? sortBy = null, bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderByDescending(x => x.BillDate);

        return sortBy.ToLower() switch
        {
            "number" => isDescending ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number),
            "billdate" => isDescending ? query.OrderByDescending(x => x.BillDate) : query.OrderBy(x => x.BillDate),
            "duedate" => isDescending ? query.OrderByDescending(x => x.DueDate) : query.OrderBy(x => x.DueDate),
            "total" => isDescending ? query.OrderByDescending(x => x.Total) : query.OrderBy(x => x.Total),
            "status" => isDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            _ => throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }

    public static IQueryable<VendorBillDto> ToVendorBillDto(this IQueryable<VendorBill> query) =>
        query.Select(ExpressionToDto());

    public static Expression<Func<VendorBill, VendorBillDto>> ExpressionToDto() => x => new VendorBillDto
    {
        Id = x.Id,
        Number = x.Number,
        SupplierNumber = x.SupplierNumber,
        PartnerId = x.PartnerId,
        PartnerName = x.Partner.Name,
        PurchaseOrderId = x.PurchaseOrderId,
        PurchaseOrderNumber = x.PurchaseOrder.Number,
        BillDate = x.BillDate,
        DueDate = x.DueDate,
        Status = x.Status,
        Currency = x.Currency,
        SubTotal = x.SubTotal,
        TaxRate = x.TaxRate,
        TaxAmount = x.TaxAmount,
        Total = x.Total,
        PaidAmount = x.PaidAmount,
        PostedAt = x.PostedAt,
        PaidAt = x.PaidAt,
        Notes = x.Notes,
        CreatedAt = x.Created,
        Lines = x.Lines.Select(l => new VendorBillLineDto
        {
            Id = l.Id,
            VendorBillId = l.VendorBillId,
            PurchaseOrderLineId = l.PurchaseOrderLineId,
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
