using System.ComponentModel.DataAnnotations;
using DongTaErp.Domain.Enums;

namespace DongTaErp.Domain.Entities;

/// <summary>
/// Đơn mua. Chứng từ thương mại, không cộng tồn.
/// WarehouseId chỉ là kho đề xuất khi tạo phiếu nhập.
/// </summary>
public class PurchaseOrder : BaseAuditableEntity
{
    [MaxLength(30)]
    public required string Number { get; set; }

    public Guid PartnerId { get; set; }
    public Partner Partner { get; set; } = null!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.Today;
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public DateTime? ConfirmedAt { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "VND";

    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; } = 0.08m;
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }

    [MaxLength(400)]
    public string? Notes { get; set; }

    public ICollection<PurchaseOrderLine> Lines { get; set; } = [];
    public ICollection<GoodsReceipt> GoodsReceipts { get; set; } = [];
    public ICollection<VendorBill> VendorBills { get; set; } = [];
}

public class PurchaseOrderLine : BaseAuditableEntity
{
    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }

    public ICollection<GoodsReceiptLine> GoodsReceiptLines { get; set; } = [];
    public ICollection<VendorBillLine> VendorBillLines { get; set; } = [];
}

/// <summary>
/// Phiếu nhập kho mua hàng. Một phiếu thuộc một đơn mua và một kho.
/// Chỉ khi Status = Posted mới cộng tồn và ghi InventoryTxn loại In.
/// </summary>
public class GoodsReceipt : BaseAuditableEntity
{
    [MaxLength(30)]
    public required string Code { get; set; }

    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public DateTime ReceiptDate { get; set; } = DateTime.Today;
    public GoodsReceiptStatus Status { get; set; } = GoodsReceiptStatus.Draft;
    public DateTime? PostedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    [MaxLength(100)]
    public string? SupplierRef { get; set; }

    [MaxLength(400)]
    public string? Notes { get; set; }

    public ICollection<GoodsReceiptLine> Lines { get; set; } = [];
}

public class GoodsReceiptLine : BaseAuditableEntity
{
    public Guid GoodsReceiptId { get; set; }
    public GoodsReceipt GoodsReceipt { get; set; } = null!;

    public Guid PurchaseOrderLineId { get; set; }
    public PurchaseOrderLine PurchaseOrderLine { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public decimal Quantity { get; set; }

    [MaxLength(300)]
    public string? Note { get; set; }
}

/// <summary>
/// Hóa đơn nhà cung cấp. Phát sinh phải trả = Total - PaidAmount.
/// Số lượng dòng không vượt số đã nhập (GoodsReceipt Posted).
/// </summary>
public class VendorBill : BaseAuditableEntity
{
    [MaxLength(30)]
    public required string Number { get; set; }

    [MaxLength(60)]
    public string? SupplierNumber { get; set; }

    public Guid PartnerId { get; set; }
    public Partner Partner { get; set; } = null!;

    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public DateTime BillDate { get; set; } = DateTime.Today;
    public DateTime? DueDate { get; set; }
    public VendorBillStatus Status { get; set; } = VendorBillStatus.Draft;

    [MaxLength(10)]
    public string Currency { get; set; } = "VND";

    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; }

    public DateTime? PostedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    [MaxLength(400)]
    public string? Notes { get; set; }

    public ICollection<VendorBillLine> Lines { get; set; } = [];
    public ICollection<DisbursementAllocation> Allocations { get; set; } = [];
}

public class VendorBillLine : BaseAuditableEntity
{
    public Guid VendorBillId { get; set; }
    public VendorBill VendorBill { get; set; } = null!;

    public Guid PurchaseOrderLineId { get; set; }
    public PurchaseOrderLine PurchaseOrderLine { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

/// <summary>
/// Phiếu chi cho nhà cung cấp. Amount là số tiền trả.
/// Phần chưa phân bổ = Amount - sum(Allocations).
/// </summary>
public class Disbursement : BaseAuditableEntity
{
    [MaxLength(30)]
    public required string Number { get; set; }

    public Guid PartnerId { get; set; }
    public Partner Partner { get; set; } = null!;

    public DateTime PaidAt { get; set; } = DateTime.Today;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.BankTransfer;
    public DisbursementStatus Status { get; set; } = DisbursementStatus.Draft;

    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }

    [MaxLength(400)]
    public string? Notes { get; set; }

    public DateTime? PostedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public ICollection<DisbursementAllocation> Allocations { get; set; } = [];
}

/// <summary>
/// Phân bổ một phiếu chi vào một hóa đơn nhà cung cấp. Nguồn sự thật của VendorBill.PaidAmount.
/// </summary>
public class DisbursementAllocation : BaseAuditableEntity
{
    public Guid DisbursementId { get; set; }
    public Disbursement Disbursement { get; set; } = null!;

    public Guid VendorBillId { get; set; }
    public VendorBill VendorBill { get; set; } = null!;

    public decimal Amount { get; set; }
}
