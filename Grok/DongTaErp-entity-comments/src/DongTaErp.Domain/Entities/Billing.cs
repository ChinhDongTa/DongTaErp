using System.ComponentModel.DataAnnotations;
using DongTaErp.Domain.Enums;

namespace DongTaErp.Domain.Entities;

/// <summary>
/// Hóa đơn bán. Công nợ khách phát sinh khi Status = Issued.
/// Còn nợ = Total - PaidAmount. PaidAmount lấy từ phiếu thu đã ghi sổ.
/// </summary>
public class Invoice : BaseAuditableEntity
{
    /// <summary>Số hóa đơn, duy nhất.</summary>
    [MaxLength(30)]
    public required string Number { get; set; }

    /// <summary>Khách hàng. Lấy từ đơn bán.</summary>
    public Guid PartnerId { get; set; }

    /// <summary>Khách hàng.</summary>
    public Partner Partner { get; set; } = null!;

    /// <summary>Đơn bán nguồn.</summary>
    public Guid SalesOrderId { get; set; }

    /// <summary>Đơn bán.</summary>
    public SalesOrder SalesOrder { get; set; } = null!;

    /// <summary>Ngày hóa đơn.</summary>
    public DateTime IssueDate { get; set; } = DateTime.Today;

    /// <summary>Hạn thanh toán. Dùng để tính quá hạn.</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>Nháp, đã phát hành, thanh toán một phần, đã thanh toán, đã hủy.</summary>
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    /// <summary>Tiền tệ, mặc định VND.</summary>
    [MaxLength(10)]
    public string Currency { get; set; } = "VND";

    /// <summary>Tổng tiền hàng trước thuế.</summary>
    public decimal SubTotal { get; set; }

    /// <summary>Thuế suất.</summary>
    public decimal TaxRate { get; set; }

    /// <summary>Tiền thuế.</summary>
    public decimal TaxAmount { get; set; }

    /// <summary>Tổng phải thu.</summary>
    public decimal Total { get; set; }

    /// <summary>Đã thu. Cache của tổng phân bổ từ phiếu thu đã ghi sổ.</summary>
    public decimal PaidAmount { get; set; }

    /// <summary>Thời điểm phát hành.</summary>
    public DateTime? IssuedAt { get; set; }

    /// <summary>Thời điểm thu đủ.</summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>Thời điểm hủy.</summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>Ghi chú.</summary>
    [MaxLength(400)]
    public string? Notes { get; set; }

    /// <summary>Các dòng hóa đơn.</summary>
    public ICollection<InvoiceLine> Lines { get; set; } = [];

    /// <summary>Các lần phân bổ phiếu thu.</summary>
    public ICollection<PaymentAllocation> Allocations { get; set; } = [];
}

/// <summary>
/// Dòng hóa đơn bán. Số lượng không vượt số đã xuất kho của dòng đơn.
/// </summary>
public class InvoiceLine : BaseAuditableEntity
{
    /// <summary>Hóa đơn.</summary>
    public Guid InvoiceId { get; set; }

    /// <summary>Hóa đơn.</summary>
    public Invoice Invoice { get; set; } = null!;

    /// <summary>Dòng đơn bán.</summary>
    public Guid SalesOrderLineId { get; set; }

    /// <summary>Dòng đơn bán.</summary>
    public SalesOrderLine SalesOrderLine { get; set; } = null!;

    /// <summary>Sản phẩm.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Sản phẩm.</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Số lượng xuất hóa đơn.</summary>
    public decimal Qty { get; set; }

    /// <summary>Đơn giá.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Thành tiền = Qty x UnitPrice.</summary>
    public decimal LineTotal { get; set; }
}

/// <summary>
/// Phiếu thu của khách. Amount là số tiền nhận.
/// Phần chưa phân bổ = Amount - tổng Allocations, không làm giảm công nợ.
/// </summary>
public class Payment : BaseAuditableEntity
{
    /// <summary>Số phiếu thu, duy nhất.</summary>
    [MaxLength(30)]
    public required string Number { get; set; }

    /// <summary>Khách nộp tiền.</summary>
    public Guid PartnerId { get; set; }

    /// <summary>Khách hàng.</summary>
    public Partner Partner { get; set; } = null!;

    /// <summary>Ngày thu tiền.</summary>
    public DateTime PaidAt { get; set; } = DateTime.Today;

    /// <summary>Số tiền nhận.</summary>
    public decimal Amount { get; set; }

    /// <summary>Hình thức: tiền mặt, chuyển khoản, thẻ, khác.</summary>
    public PaymentMethod Method { get; set; } = PaymentMethod.BankTransfer;

    /// <summary>Nháp, đã ghi sổ, hoặc đã hủy.</summary>
    public CustomerPaymentStatus Status { get; set; } = CustomerPaymentStatus.Draft;

    /// <summary>Số tham chiếu ngân hàng hoặc biên lai.</summary>
    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }

    /// <summary>Ghi chú.</summary>
    [MaxLength(400)]
    public string? Notes { get; set; }

    /// <summary>Thời điểm ghi sổ.</summary>
    public DateTime? PostedAt { get; set; }

    /// <summary>Thời điểm hủy.</summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>Phân bổ vào các hóa đơn.</summary>
    public ICollection<PaymentAllocation> Allocations { get; set; } = [];
}

/// <summary>
/// Phân bổ một phiếu thu vào một hóa đơn. Đây là nguồn sự thật của Invoice.PaidAmount.
/// </summary>
public class PaymentAllocation : BaseAuditableEntity
{
    /// <summary>Phiếu thu.</summary>
    public Guid PaymentId { get; set; }

    /// <summary>Phiếu thu.</summary>
    public Payment Payment { get; set; } = null!;

    /// <summary>Hóa đơn được trừ nợ.</summary>
    public Guid InvoiceId { get; set; }

    /// <summary>Hóa đơn.</summary>
    public Invoice Invoice { get; set; } = null!;

    /// <summary>Số tiền phân bổ. Không vượt số còn nợ của hóa đơn.</summary>
    public decimal Amount { get; set; }
}
