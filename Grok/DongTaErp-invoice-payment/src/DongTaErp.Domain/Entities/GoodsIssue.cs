using System.ComponentModel.DataAnnotations;
using DongTaErp.Domain.Enums;

namespace DongTaErp.Domain.Entities;

/// <summary>
/// Phiếu xuất kho. Một phiếu thuộc một đơn và một kho.
/// Chỉ khi Status = Posted mới trừ tồn và ghi InventoryTxn.
/// </summary>
public class GoodsIssue : BaseAuditableEntity
{
    [MaxLength(30)]
    public required string Code { get; set; }

    public Guid SalesOrderId { get; set; }
    public SalesOrder SalesOrder { get; set; } = null!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public DateTime IssueDate { get; set; }
    public GoodsIssueStatus Status { get; set; } = GoodsIssueStatus.Draft;
    public DateTime? PostedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    [MaxLength(400)]
    public string? Notes { get; set; }

    public ICollection<GoodsIssueLine> Lines { get; set; } = [];
}
