namespace DongTaErp.Application.Services.Invoices;

internal static class Extensions
{
    public static IQueryable<Invoice> ApplySorting(this IQueryable<Invoice> query, string? sortBy = null, bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderByDescending(x => x.IssueDate);

        return sortBy.ToLower() switch
        {
            "number" => isDescending ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number),
            "issuedate" => isDescending ? query.OrderByDescending(x => x.IssueDate) : query.OrderBy(x => x.IssueDate),
            "duedate" => isDescending ? query.OrderByDescending(x => x.DueDate) : query.OrderBy(x => x.DueDate),
            "total" => isDescending ? query.OrderByDescending(x => x.Total) : query.OrderBy(x => x.Total),
            "status" => isDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            _ => throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }

    public static IQueryable<InvoiceDto> ToInvoiceDto(this IQueryable<Invoice> query) =>
        query.Select(ExpressionToDto());

    public static Expression<Func<Invoice, InvoiceDto>> ExpressionToDto() => x => new InvoiceDto
    {
        Id = x.Id,
        Number = x.Number,
        PartnerId = x.PartnerId,
        PartnerName = x.Partner.Name,
        SalesOrderId = x.SalesOrderId,
        SalesOrderNumber = x.SalesOrder.Number,
        IssueDate = x.IssueDate,
        DueDate = x.DueDate,
        Status = x.Status,
        Currency = x.Currency,
        SubTotal = x.SubTotal,
        TaxRate = x.TaxRate,
        TaxAmount = x.TaxAmount,
        Total = x.Total,
        PaidAmount = x.PaidAmount,
        IssuedAt = x.IssuedAt,
        PaidAt = x.PaidAt,
        Notes = x.Notes,
        CreatedAt = x.Created,
        Lines = x.Lines.Select(l => new InvoiceLineDto
        {
            Id = l.Id,
            InvoiceId = l.InvoiceId,
            SalesOrderLineId = l.SalesOrderLineId,
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
