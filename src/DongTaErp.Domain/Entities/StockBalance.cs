namespace DongTaErp.Domain.Entities;

/// <summary>
/// Tồn kho hiện tại của một sản phẩm tại một kho.
/// Không sửa tay. Chỉ đổi khi phiếu nhập/xuất được ghi sổ hoặc hủy.
/// </summary>
public class StockBalance : BaseAuditableEntity
{
    /// <summary>Sản phẩm.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Sản phẩm.</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Kho.</summary>
    public Guid WarehouseId { get; set; }

    /// <summary>Kho.</summary>
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>Số lượng tồn hiện tại.</summary>
    public decimal Qty { get; set; }

    /// <summary>Tồn tối thiểu tại kho này, dùng cảnh báo.</summary>
    public decimal MinQty { get; set; }
}
