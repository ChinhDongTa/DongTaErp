namespace DongTaErp.Application.Services.GoodsIssues;

internal static class Extensions
{
    public static IQueryable<GoodsIssue> ApplySorting(this IQueryable<GoodsIssue> query, string? sortBy = null, bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderByDescending(x => x.IssueDate);

        return sortBy.ToLower() switch
        {
            "code" => isDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
            "issuedate" => isDescending ? query.OrderByDescending(x => x.IssueDate) : query.OrderBy(x => x.IssueDate),
            "status" => isDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            _ => throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }

    public static IQueryable<GoodsIssueDto> ToGoodsIssueDto(this IQueryable<GoodsIssue> query) =>
        query.Select(ExpressionToDto());

    public static Expression<Func<GoodsIssue, GoodsIssueDto>> ExpressionToDto() => x => new GoodsIssueDto
    {
        Id = x.Id,
        Code = x.Code,
        SalesOrderId = x.SalesOrderId,
        SalesOrderNumber = x.SalesOrder.Number,
        WarehouseId = x.WarehouseId,
        WarehouseName = x.Warehouse.Name,
        IssueDate = x.IssueDate,
        Status = x.Status,
        PostedAt = x.PostedAt,
        CancelledAt = x.CancelledAt,
        Notes = x.Notes,
        CreatedAt = x.Created,
        Lines = x.Lines.Select(l => new GoodsIssueLineDto
        {
            Id = l.Id,
            GoodsIssueId = l.GoodsIssueId,
            SalesOrderLineId = l.SalesOrderLineId,
            ProductId = l.ProductId,
            ProductSku = l.Product.Sku,
            ProductName = l.Product.Name,
            Quantity = l.Quantity,
            Note = l.Note,
            CreatedAt = l.Created
        }).ToList()
    };
}
