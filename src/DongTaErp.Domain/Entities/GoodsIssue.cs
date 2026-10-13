using System.ComponentModel.DataAnnotations;
using DongTaErp.Domain.Enums;

namespace DongTaErp.Domain.Entities;

/// <summary>
/// Phiếu xuất kho bán hàng. Một phiếu thuộc một đơn và một kho.
/// Chỉ khi Status = Posted mới trừ tồn và ghi InventoryTxn.
/// </summary>
public class GoodsIssue : BaseAuditableEntity
{
    /// <summary>Mã phiếu, duy nhất.</summary>
    [MaxLength(30)]
    public required string Code { get; set; }

    /// <summary>Đơn bán được xuất.</summary>
    public Guid SalesOrderId { get; set; }

    /// <summary>Đơn bán.</summary>
    public SalesOrder SalesOrder { get; set; } = null!;

    /// <summary>Kho xuất thật. Tồn bị trừ ở kho này khi ghi sổ.</summary>
    public Guid WarehouseId { get; set; }

    /// <summary>Kho xuất.</summary>
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>Ngày xuất.</summary>
    public DateTime IssueDate { get; set; }

    /// <summary>Nháp, đã xuất, hoặc đã hủy.</summary>
    public GoodsIssueStatus Status { get; set; } = GoodsIssueStatus.Draft;

    /// <summary>Thời điểm ghi sổ, lúc trừ kho.</summary>
    public DateTime? PostedAt { get; set; }

    /// <summary>Thời điểm hủy.</summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>Ghi chú.</summary>
    [MaxLength(400)]
    public string? Notes { get; set; }

    /// <summary>Các dòng xuất.</summary>
    public ICollection<GoodsIssueLine> Lines { get; set; } = [];
}
