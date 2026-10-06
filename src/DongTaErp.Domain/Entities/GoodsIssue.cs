namespace DongTaErp.Domain.Entities;

/// <summary>
/// Phiếu xuất kho
/// </summary>
public class GoodsIssue : BaseAuditableEntity
{
    public required string Code { get; set; } 

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public DateTime IssueDate { get; set; }

    public ICollection<GoodsIssueLine> Lines { get; set; } = [];
}