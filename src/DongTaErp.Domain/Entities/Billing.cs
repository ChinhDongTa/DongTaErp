using DongTaErp.Domain.Enums;

namespace DongTaErp.Domain.Entities;

/// <summary>
/// Hóa đơn bán. Công nợ khách = Total - PaidAmount.
/// PaidAmount là tổng phân bổ từ Payment đã Posted.
/// </summary>
public class Invoice : BaseAuditableEntity
{
    [MaxLength(30)]
    public required string Number { get; set; }

    public Guid PartnerId { get; set; }
    public Partner Partner { get; set; } = null!;

    public Guid SalesOrderId { get; set; }
    public SalesOrder SalesOrder { get; set; } = null!;

    public DateTime IssueDate { get; set; } = DateTime.Today;
    public DateTime? DueDate { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    [MaxLength(10)]
    public string Currency { get; set; } = "VND";

    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; }

    public DateTime? IssuedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    [MaxLength(400)]
    public string? Notes { get; set; }

    public ICollection<InvoiceLine> Lines { get; set; } = [];
    public ICollection<PaymentAllocation> Allocations { get; set; } = [];
}

public class InvoiceLine : BaseAuditableEntity
{
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;

    public Guid SalesOrderLineId { get; set; }
    public SalesOrderLine SalesOrderLine { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

/// <summary>
/// Phiếu thu của khách. Amount là số tiền nhận. Phần chưa phân bổ = Amount - sum(Allocations).
/// </summary>
public class Payment : BaseAuditableEntity
{
    [MaxLength(30)]
    public required string Number { get; set; }

    public Guid PartnerId { get; set; }
    public Partner Partner { get; set; } = null!;

    public DateTime PaidAt { get; set; } = DateTime.Today;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.BankTransfer;
    public CustomerPaymentStatus Status { get; set; } = CustomerPaymentStatus.Draft;

    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }

    [MaxLength(400)]
    public string? Notes { get; set; }

    public DateTime? PostedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public ICollection<PaymentAllocation> Allocations { get; set; } = [];
}

/// <summary>
/// Phân bổ một phiếu thu vào một hóa đơn. Nguồn sự thật của PaidAmount.
/// </summary>
public class PaymentAllocation : BaseAuditableEntity
{
    public Guid PaymentId { get; set; }
    public Payment Payment { get; set; } = null!;

    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;

    public decimal Amount { get; set; }
}
