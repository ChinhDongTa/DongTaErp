using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Common.DTOs;

public class InventoryTxnDto : BaseDto
{
    public string ProductSku { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string WarehouseName { get; set; } = null!;

    public string? ContraWarehouseName { get; set; }
    public InventoryTxnType Type { get; set; }
    public string TypeName => Type.ToDisplayName();

    public decimal QtyChange { get; set; }

    public string Reference { get; set; } = null!;

    public string? Note { get; set; }
}

public class CreateInventoryTxnDto
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
    /// Kho đối ứng.
    /// Không bắt buộc.
    /// </summary>
    public Guid? ContraWarehouseId { get; set; }

    /// <summary>
    /// Loại giao dịch kho.
    /// Bắt buộc.
    /// Giá trị phải thuộc enum InventoryTxnType.
    /// </summary>
    public InventoryTxnType Type { get; set; }

    /// <summary>
    /// Số lượng thay đổi.
    /// Khác 0.
    /// </summary>
    public decimal QtyChange { get; set; }

    /// <summary>
    /// Chứng từ tham chiếu.
    /// Bắt buộc.
    /// Tối đa 80 ký tự.
    /// </summary>
    public required string Reference { get; set; }

    /// <summary>
    /// Ghi chú.
    /// Không bắt buộc.
    /// Tối đa 300 ký tự.
    /// </summary>
    public string? Note { get; set; }
}

public class UpdateInventoryTxnDto
{
    public Guid Id { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? WarehouseId { get; set; }

    public Guid? ContraWarehouseId { get; set; }

    public InventoryTxnType? Type { get; set; }

    public decimal? QtyChange { get; set; }

    public string? Reference { get; set; }

    public string? Note { get; set; }
}