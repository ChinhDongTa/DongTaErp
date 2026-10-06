namespace DongTaErp.Application.Common.DTOs;

public class StockBalanceDto : BaseDto
{
    public string ProductSku { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string WarehouseName { get; set; } = null!;

    public decimal Qty { get; set; }

    public decimal MinQty { get; set; }
}

public class CreateStockBalanceDto
{
    /// <summary>
    /// Sản phẩm.
    /// Bắt buộc.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Kho.
    /// Bắt buộc.
    /// </summary>
    public Guid WarehouseId { get; set; }

    /// <summary>
    /// Tồn kho hiện tại.
    /// Phải lớn hơn hoặc bằng 0.
    /// </summary>
    public decimal Qty { get; set; }

    /// <summary>
    /// Tồn kho tối thiểu.
    /// Phải lớn hơn hoặc bằng 0.
    /// </summary>
    public decimal MinQty { get; set; }
}

public class UpdateStockBalanceDto
{
    public Guid Id { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? WarehouseId { get; set; }

    public decimal? Qty { get; set; }

    public decimal? MinQty { get; set; }
}