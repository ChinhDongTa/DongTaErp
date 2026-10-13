using System.ComponentModel.DataAnnotations;

namespace DongTaErp.Domain.Entities;

/// <summary>
/// Dòng phiếu xuất. Quantity là số xuất của một dòng đơn, từ kho của phiếu.
/// </summary>
public class GoodsIssueLine : BaseAuditableEntity
{
    /// <summary>Phiếu xuất.</summary>
    public Guid GoodsIssueId { get; set; }

    /// <summary>Phiếu xuất.</summary>
    public GoodsIssue GoodsIssue { get; set; } = null!;

    /// <summary>Dòng đơn bán được xuất.</summary>
    public Guid SalesOrderLineId { get; set; }

    /// <summary>Dòng đơn bán.</summary>
    public SalesOrderLine SalesOrderLine { get; set; } = null!;

    /// <summary>Sản phẩm. Phải khớp sản phẩm của dòng đơn.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Sản phẩm.</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Số lượng xuất. Không vượt phần còn lại của dòng đơn.</summary>
    public decimal Quantity { get; set; }

    /// <summary>Ghi chú dòng.</summary>
    [MaxLength(300)]
    public string? Note { get; set; }
}
