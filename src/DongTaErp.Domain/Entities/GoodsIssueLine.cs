namespace DongTaErp.Domain.Entities;

/// <summary>
/// Chi tiết phiếu xuất kho
/// </summary>
public class GoodsIssueLine: BaseAuditableEntity
{
    public Guid GoodsIssueId { get; set; }
    public GoodsIssue GoodsIssue { get; set; } = null!;

    public Guid SalesOrderLineId { get; set; }
    public SalesOrderLine SalesOrderLine { get; set; } = null!;

    public decimal Quantity { get; set; }

    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;
}