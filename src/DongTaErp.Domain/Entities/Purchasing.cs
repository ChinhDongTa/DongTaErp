using System.ComponentModel.DataAnnotations;
using DongTaErp.Domain.Enums;

namespace DongTaErp.Domain.Entities;

/// <summary>
/// Đơn mua. Chứng từ thương mại, không cộng tồn.
/// WarehouseId chỉ là kho đề xuất khi tạo phiếu nhập.
/// </summary>
public class PurchaseOrder : BaseAuditableEntity
{
    /// <summary>Số đơn mua, duy nhất.</summary>
    [MaxLength(30)]
    public required string Number { get; set; }

    /// <summary>Nhà cung cấp.</summary>
    public Guid PartnerId { get; set; }

    /// <summary>Nhà cung cấp.</summary>
    public Partner Partner { get; set; } = null!;

    /// <summary>Kho đề xuất khi nhập. Kho nhập thật nằm trên phiếu nhập.</summary>
    public Guid WarehouseId { get; set; }

    /// <summary>Kho đề xuất.</summary>
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>Ngày đơn.</summary>
    public DateTime OrderDate { get; set; } = DateTime.Today;

    /// <summary>Nháp, đã xác nhận, hoặc đã hủy.</summary>
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;

    /// <summary>Thời điểm xác nhận.</summary>
    public DateTime? ConfirmedAt { get; set; }

    /// <summary>Tiền tệ, mặc định VND.</summary>
    [MaxLength(10)]
    public string Currency { get; set; } = "VND";

    /// <summary>Tổng tiền hàng trước thuế.</summary>
    public decimal SubTotal { get; set; }

    /// <summary>Thuế suất.</summary>
    public decimal TaxRate { get; set; } = 0.08m;

    /// <summary>Tiền thuế.</summary>
    public decimal TaxAmount { get; set; }

    /// <summary>Tổng thanh toán.</summary>
    public decimal Total { get; set; }

    /// <summary>Ghi chú.</summary>
    [MaxLength(400)]
    public string? Notes { get; set; }

    /// <summary>Các dòng hàng.</summary>
    public ICollection<PurchaseOrderLine> Lines { get; set; } = [];

    /// <summary>Các phiếu nhập của đơn.</summary>
    public ICollection<GoodsReceipt> GoodsReceipts { get; set; } = [];

    /// <summary>Các hóa đơn nhà cung cấp của đơn.</summary>
    public ICollection<VendorBill> VendorBills { get; set; } = [];
}

/// <summary>
/// Dòng đơn mua. Không chứa kho. Số đã nhập nằm ở GoodsReceiptLine.
/// </summary>
public class PurchaseOrderLine : BaseAuditableEntity
{
    /// <summary>Đơn mua.</summary>
    public Guid PurchaseOrderId { get; set; }

    /// <summary>Đơn mua.</summary>
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    /// <summary>Sản phẩm.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Sản phẩm.</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Số lượng đặt.</summary>
    public decimal Qty { get; set; }

    /// <summary>Đơn giá mua.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Thành tiền = Qty x UnitPrice.</summary>
    public decimal LineTotal { get; set; }

    /// <summary>Các lần nhập kho của dòng này.</summary>
    public ICollection<GoodsReceiptLine> GoodsReceiptLines { get; set; } = [];

    /// <summary>Các lần ghi hóa đơn NCC của dòng này.</summary>
    public ICollection<VendorBillLine> VendorBillLines { get; set; } = [];
}

/// <summary>
/// Phiếu nhập kho mua hàng. Một phiếu thuộc một đơn mua và một kho.
/// Chỉ khi Status = Posted mới cộng tồn và ghi InventoryTxn loại In.
/// </summary>
public class GoodsReceipt : BaseAuditableEntity
{
    /// <summary>Mã phiếu, duy nhất.</summary>
    [MaxLength(30)]
    public required string Code { get; set; }

    /// <summary>Đơn mua được nhập.</summary>
    public Guid PurchaseOrderId { get; set; }

    /// <summary>Đơn mua.</summary>
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    /// <summary>Kho nhập thật.</summary>
    public Guid WarehouseId { get; set; }

    /// <summary>Kho nhập.</summary>
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>Ngày nhập.</summary>
    public DateTime ReceiptDate { get; set; } = DateTime.Today;

    /// <summary>Nháp, đã nhập, hoặc đã hủy.</summary>
    public GoodsReceiptStatus Status { get; set; } = GoodsReceiptStatus.Draft;

    /// <summary>Thời điểm ghi sổ, lúc cộng kho.</summary>
    public DateTime? PostedAt { get; set; }

    /// <summary>Thời điểm hủy.</summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>Số chứng từ của nhà cung cấp, phiếu giao hàng.</summary>
    [MaxLength(100)]
    public string? SupplierRef { get; set; }

    /// <summary>Ghi chú.</summary>
    [MaxLength(400)]
    public string? Notes { get; set; }

    /// <summary>Các dòng nhập.</summary>
    public ICollection<GoodsReceiptLine> Lines { get; set; } = [];
}

/// <summary>
/// Dòng phiếu nhập. Quantity là số nhập của một dòng đơn mua.
/// </summary>
public class GoodsReceiptLine : BaseAuditableEntity
{
    /// <summary>Phiếu nhập.</summary>
    public Guid GoodsReceiptId { get; set; }

    /// <summary>Phiếu nhập.</summary>
    public GoodsReceipt GoodsReceipt { get; set; } = null!;

    /// <summary>Dòng đơn mua.</summary>
    public Guid PurchaseOrderLineId { get; set; }

    /// <summary>Dòng đơn mua.</summary>
    public PurchaseOrderLine PurchaseOrderLine { get; set; } = null!;

    /// <summary>Sản phẩm. Phải khớp sản phẩm của dòng đơn.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Sản phẩm.</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Số lượng nhập. Không vượt phần còn lại của dòng đơn.</summary>
    public decimal Quantity { get; set; }

    /// <summary>Ghi chú dòng.</summary>
    [MaxLength(300)]
    public string? Note { get; set; }
}

/// <summary>
/// Hóa đơn nhà cung cấp. Phát sinh phải trả khi Status = Posted.
/// Còn nợ = Total - PaidAmount. Số lượng dòng không vượt số đã nhập.
/// </summary>
public class VendorBill : BaseAuditableEntity
{
    /// <summary>Số hóa đơn nội bộ, duy nhất.</summary>
    [MaxLength(30)]
    public required string Number { get; set; }

    /// <summary>Số hóa đơn trên chứng từ của nhà cung cấp.</summary>
    [MaxLength(60)]
    public string? SupplierNumber { get; set; }

    /// <summary>Nhà cung cấp.</summary>
    public Guid PartnerId { get; set; }

    /// <summary>Nhà cung cấp.</summary>
    public Partner Partner { get; set; } = null!;

    /// <summary>Đơn mua nguồn.</summary>
    public Guid PurchaseOrderId { get; set; }

    /// <summary>Đơn mua.</summary>
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    /// <summary>Ngày hóa đơn.</summary>
    public DateTime BillDate { get; set; } = DateTime.Today;

    /// <summary>Hạn thanh toán.</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>Nháp, đã ghi sổ, thanh toán một phần, đã thanh toán, đã hủy.</summary>
    public VendorBillStatus Status { get; set; } = VendorBillStatus.Draft;

    /// <summary>Tiền tệ.</summary>
    [MaxLength(10)]
    public string Currency { get; set; } = "VND";

    /// <summary>Tổng tiền hàng trước thuế.</summary>
    public decimal SubTotal { get; set; }

    /// <summary>Thuế suất.</summary>
    public decimal TaxRate { get; set; }

    /// <summary>Tiền thuế.</summary>
    public decimal TaxAmount { get; set; }

    /// <summary>Tổng phải trả.</summary>
    public decimal Total { get; set; }

    /// <summary>Đã trả. Cache của tổng phân bổ từ phiếu chi đã ghi sổ.</summary>
    public decimal PaidAmount { get; set; }

    /// <summary>Thời điểm ghi sổ.</summary>
    public DateTime? PostedAt { get; set; }

    /// <summary>Thời điểm trả đủ.</summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>Thời điểm hủy.</summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>Ghi chú.</summary>
    [MaxLength(400)]
    public string? Notes { get; set; }

    /// <summary>Các dòng hóa đơn.</summary>
    public ICollection<VendorBillLine> Lines { get; set; } = [];

    /// <summary>Các lần phân bổ phiếu chi.</summary>
    public ICollection<DisbursementAllocation> Allocations { get; set; } = [];
}

/// <summary>
/// Dòng hóa đơn nhà cung cấp. Số lượng không vượt số đã nhập kho.
/// </summary>
public class VendorBillLine : BaseAuditableEntity
{
    /// <summary>Hóa đơn nhà cung cấp.</summary>
    public Guid VendorBillId { get; set; }

    /// <summary>Hóa đơn nhà cung cấp.</summary>
    public VendorBill VendorBill { get; set; } = null!;

    /// <summary>Dòng đơn mua.</summary>
    public Guid PurchaseOrderLineId { get; set; }

    /// <summary>Dòng đơn mua.</summary>
    public PurchaseOrderLine PurchaseOrderLine { get; set; } = null!;

    /// <summary>Sản phẩm.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Sản phẩm.</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Số lượng ghi nhận công nợ.</summary>
    public decimal Qty { get; set; }

    /// <summary>Đơn giá.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Thành tiền = Qty x UnitPrice.</summary>
    public decimal LineTotal { get; set; }
}

/// <summary>
/// Phiếu chi cho nhà cung cấp. Amount là số tiền trả.
/// Phần chưa phân bổ không làm giảm công nợ.
/// </summary>
public class Disbursement : BaseAuditableEntity
{
    /// <summary>Số phiếu chi, duy nhất.</summary>
    [MaxLength(30)]
    public required string Number { get; set; }

    /// <summary>Nhà cung cấp nhận tiền.</summary>
    public Guid PartnerId { get; set; }

    /// <summary>Nhà cung cấp.</summary>
    public Partner Partner { get; set; } = null!;

    /// <summary>Ngày chi tiền.</summary>
    public DateTime PaidAt { get; set; } = DateTime.Today;

    /// <summary>Số tiền chi.</summary>
    public decimal Amount { get; set; }

    /// <summary>Hình thức chi.</summary>
    public PaymentMethod Method { get; set; } = PaymentMethod.BankTransfer;

    /// <summary>Nháp, đã ghi sổ, hoặc đã hủy.</summary>
    public DisbursementStatus Status { get; set; } = DisbursementStatus.Draft;

    /// <summary>Số tham chiếu ngân hàng hoặc ủy nhiệm chi.</summary>
    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }

    /// <summary>Ghi chú.</summary>
    [MaxLength(400)]
    public string? Notes { get; set; }

    /// <summary>Thời điểm ghi sổ.</summary>
    public DateTime? PostedAt { get; set; }

    /// <summary>Thời điểm hủy.</summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>Phân bổ vào các hóa đơn nhà cung cấp.</summary>
    public ICollection<DisbursementAllocation> Allocations { get; set; } = [];
}

/// <summary>
/// Phân bổ một phiếu chi vào một hóa đơn nhà cung cấp.
/// Đây là nguồn sự thật của VendorBill.PaidAmount.
/// </summary>
public class DisbursementAllocation : BaseAuditableEntity
{
    /// <summary>Phiếu chi.</summary>
    public Guid DisbursementId { get; set; }

    /// <summary>Phiếu chi.</summary>
    public Disbursement Disbursement { get; set; } = null!;

    /// <summary>Hóa đơn nhà cung cấp được trừ nợ.</summary>
    public Guid VendorBillId { get; set; }

    /// <summary>Hóa đơn nhà cung cấp.</summary>
    public VendorBill VendorBill { get; set; } = null!;

    /// <summary>Số tiền phân bổ. Không vượt số còn nợ.</summary>
    public decimal Amount { get; set; }
}
