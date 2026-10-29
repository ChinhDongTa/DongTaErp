namespace DongTaErp.Domain.Entities;

using DongTaErp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Đối tác. Một pháp nhân có thể vừa là khách vừa là nhà cung cấp.
/// Không tách bảng Customer / Supplier.
/// </summary>
public class Partner : BaseAuditableEntity
{
    /// <summary>Mã đối tác, duy nhất.</summary>
    [MaxLength(40)]
    public required string Code { get; set; }

    /// <summary>Tên đối tác.</summary>
    [MaxLength(200)]
    public required string Name { get; set; }

    /// <summary>Vai trò: khách, nhà cung cấp, hoặc cả hai.</summary>
    public PartnerType Type { get; set; } = PartnerType.Customer;

    /// <summary>Số điện thoại.</summary>
    [MaxLength(40)]
    public string? Phone { get; set; }

    /// <summary>Email.</summary>
    [MaxLength(160)]
    public string? Email { get; set; }

    /// <summary>Mã số thuế.</summary>
    [MaxLength(40)]
    public string? TaxCode { get; set; }

    /// <summary>Địa chỉ.</summary>
    [MaxLength(300)]
    public string? Address { get; set; }

    /// <summary>Còn sử dụng hay không.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Các đơn bán của đối tác.</summary>
    public ICollection<SalesOrder> SalesOrders { get; set; } = [];

    /// <summary>Các hóa đơn bán của đối tác.</summary>
    public ICollection<Invoice> Invoices { get; set; } = [];

    /// <summary>Các phiếu thu của đối tác.</summary>
    public ICollection<Payment> Payments { get; set; } = [];

    /// <summary>Các đơn mua của đối tác.</summary>
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = [];

    /// <summary>Các hóa đơn nhà cung cấp.</summary>
    public ICollection<VendorBill> VendorBills { get; set; } = [];

    /// <summary>Các phiếu chi cho đối tác.</summary>
    public ICollection<Disbursement> Disbursements { get; set; } = [];
}