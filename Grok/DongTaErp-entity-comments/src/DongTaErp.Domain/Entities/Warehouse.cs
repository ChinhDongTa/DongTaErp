namespace DongTaErp.Domain.Entities;

using DongTaErp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Kho hàng. Hệ thống hỗ trợ nhiều kho, có thể khác quốc gia.
/// </summary>
public class Warehouse : BaseAuditableEntity
{
    /// <summary>Mã kho, duy nhất.</summary>
    [MaxLength(30)]
    public required string Code { get; set; }

    /// <summary>Tên kho.</summary>
    [MaxLength(160)]
    public required string Name { get; set; }

    /// <summary>Loại kho: chính, chi nhánh, trung chuyển, ảo.</summary>
    public WarehouseType Type { get; set; } = WarehouseType.Branch;

    /// <summary>Mã quốc gia ISO 2 ký tự, ví dụ VN.</summary>
    [MaxLength(2)]
    public required string CountryCode { get; set; } = "VN";

    /// <summary>Thành phố.</summary>
    [MaxLength(80)]
    public string? City { get; set; }

    /// <summary>Địa chỉ kho.</summary>
    [MaxLength(300)]
    public string? Address { get; set; }

    /// <summary>Múi giờ của kho, ví dụ Asia/Ho_Chi_Minh.</summary>
    [MaxLength(64)]
    public string? TimeZone { get; set; } = "Asia/Ho_Chi_Minh";

    /// <summary>Kho mặc định khi tạo chứng từ mới.</summary>
    public bool IsDefault { get; set; }

    /// <summary>Kho còn hoạt động hay không.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Tồn của các sản phẩm trong kho.</summary>
    public ICollection<StockBalance> StockBalances { get; set; } = [];

    /// <summary>Bút toán kho phát sinh tại kho này.</summary>
    public ICollection<InventoryTxn> InventoryTxns { get; set; } = [];

    /// <summary>Đơn bán lấy kho này làm kho đề xuất.</summary>
    public ICollection<SalesOrder> SalesOrders { get; set; } = [];

    /// <summary>Phiếu xuất từ kho này.</summary>
    public ICollection<GoodsIssue> GoodsIssues { get; set; } = [];

    /// <summary>Đơn mua lấy kho này làm kho đề xuất.</summary>
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = [];

    /// <summary>Phiếu nhập vào kho này.</summary>
    public ICollection<GoodsReceipt> GoodsReceipts { get; set; } = [];
}
