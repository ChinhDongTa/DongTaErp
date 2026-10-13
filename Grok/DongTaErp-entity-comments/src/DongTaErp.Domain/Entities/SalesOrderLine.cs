namespace DongTaErp.Domain.Entities;

/// <summary>
/// Dòng đơn bán. Nhu cầu bán: sản phẩm, số lượng, đơn giá.
/// Không chứa kho. Số đã xuất nằm ở GoodsIssueLine.
/// </summary>
public class SalesOrderLine : BaseAuditableEntity
{
    /// <summary>Đơn bán.</summary>
    public Guid SalesOrderId { get; set; }

    /// <summary>Đơn bán.</summary>
    public SalesOrder SalesOrder { get; set; } = null!;

    /// <summary>Sản phẩm.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Sản phẩm.</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Số lượng đặt.</summary>
    public decimal Qty { get; set; }

    /// <summary>Đơn giá bán.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Thành tiền = Qty x UnitPrice.</summary>
    public decimal LineTotal { get; set; }

    /// <summary>Các lần xuất kho của dòng này.</summary>
    public ICollection<GoodsIssueLine> GoodsIssueLines { get; set; } = [];

    /// <summary>Các lần xuất hóa đơn của dòng này.</summary>
    public ICollection<InvoiceLine> InvoiceLines { get; set; } = [];
}
