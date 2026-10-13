namespace DongTaErp.Domain.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Sản phẩm. Tồn không nằm ở đây mà ở StockBalance theo từng kho.
/// </summary>
public class Product : BaseAuditableEntity
{
    /// <summary>Mã sản phẩm, duy nhất.</summary>
    [MaxLength(40)]
    public required string Sku { get; set; }

    /// <summary>Tên sản phẩm.</summary>
    [MaxLength(200)]
    public required string Name { get; set; }

    /// <summary>Danh mục sản phẩm.</summary>
    public Guid? CategoryId { get; set; }

    /// <summary>Danh mục sản phẩm.</summary>
    public Category? Category { get; set; }

    /// <summary>Đơn vị tính, ví dụ Cái, Kg, Thùng.</summary>
    [MaxLength(20)]
    public required string Unit { get; set; } = "Cái";

    /// <summary>Giá vốn tham chiếu. Giá nhập thật nằm trên đơn mua / hóa đơn NCC.</summary>
    public decimal CostPrice { get; set; }

    /// <summary>Giá bán tham chiếu. Giá bán thật nằm trên dòng đơn.</summary>
    public decimal SalePrice { get; set; }

    /// <summary>Tồn tối thiểu toàn hệ thống, dùng cảnh báo.</summary>
    public decimal MinStock { get; set; }

    /// <summary>Còn kinh doanh hay không.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Các dòng đơn bán của sản phẩm.</summary>
    public ICollection<SalesOrderLine> SalesLines { get; set; } = [];

    /// <summary>Các bút toán kho của sản phẩm.</summary>
    public ICollection<InventoryTxn> InventoryTxns { get; set; } = [];

    /// <summary>Tồn theo từng kho.</summary>
    public ICollection<StockBalance> StockBalances { get; set; } = [];
}
