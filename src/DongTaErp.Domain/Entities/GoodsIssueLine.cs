namespace DongTaErp.Domain.Entities;

/// <summary>
/// Dòng phiếu xuất. Quantity là số đã/sẽ xuất của một dòng đơn, từ kho của phiếu.
/// </summary>
public class GoodsIssueLine : BaseAuditableEntity
{
    public Guid GoodsIssueId { get; set; }
    public GoodsIssue GoodsIssue { get; set; } = null!;

    public Guid SalesOrderLineId { get; set; }
    public SalesOrderLine SalesOrderLine { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public decimal Quantity { get; set; }

    [MaxLength(300)]
    public string? Note { get; set; }
}
