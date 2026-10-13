namespace DongTaErp.Domain.Entities;

using DongTaErp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Đơn bán. Chứng từ thương mại: khách mua gì, giá bao nhiêu.
/// Xác nhận đơn không trừ kho. Kho xuất thật nằm trên phiếu xuất.
/// </summary>
public class SalesOrder : BaseAuditableEntity
{
    /// <summary>Số đơn, duy nhất.</summary>
    [MaxLength(30)]
    public required string Number { get; set; }

    /// <summary>Khách hàng.</summary>
    public Guid PartnerId { get; set; }

    /// <summary>Khách hàng.</summary>
    public Partner Partner { get; set; } = null!;

    /// <summary>Kho đề xuất khi tạo phiếu xuất. Không phải kho trừ tồn.</summary>
    public Guid WarehouseId { get; set; }

    /// <summary>Kho đề xuất.</summary>
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>Ngày đơn.</summary>
    public DateTime OrderDate { get; set; } = DateTime.Today;

    /// <summary>Nháp, đã xác nhận, hoặc đã hủy.</summary>
    public SalesOrderStatus Status { get; set; } = SalesOrderStatus.Draft;

    /// <summary>Ghi chú.</summary>
    [MaxLength(400)]
    public string? Notes { get; set; }

    /// <summary>Tổng tiền hàng trước thuế.</summary>
    public decimal SubTotal { get; set; }

    /// <summary>Thuế suất, ví dụ 0.08 = 8%.</summary>
    public decimal TaxRate { get; set; } = 0.08m;

    /// <summary>Tiền thuế.</summary>
    public decimal TaxAmount { get; set; }

    /// <summary>Tổng thanh toán = SubTotal + TaxAmount.</summary>
    public decimal Total { get; set; }

    /// <summary>Thời điểm xác nhận đơn.</summary>
    public DateTime? ConfirmedAt { get; set; }

    /// <summary>Các dòng hàng.</summary>
    public ICollection<SalesOrderLine> Lines { get; set; } = [];

    /// <summary>Các phiếu xuất của đơn.</summary>
    public ICollection<GoodsIssue> GoodsIssues { get; set; } = [];

    /// <summary>Các hóa đơn của đơn.</summary>
    public ICollection<Invoice> Invoices { get; set; } = [];
}
