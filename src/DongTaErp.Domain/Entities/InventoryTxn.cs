namespace DongTaErp.Domain.Entities;

using DongTaErp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Sổ kho. Mỗi lần nhập, xuất, điều chỉnh hoặc chuyển kho ghi một dòng.
/// QtyChange dương là tăng tồn, âm là giảm tồn.
/// </summary>
public class InventoryTxn : BaseAuditableEntity
{
    /// <summary>Sản phẩm.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Sản phẩm.</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Kho chịu ảnh hưởng tồn.</summary>
    public Guid WarehouseId { get; set; }

    /// <summary>Kho chịu ảnh hưởng tồn.</summary>
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>Kho đối ứng khi chuyển kho. null nếu không phải chuyển kho.</summary>
    public Guid? ContraWarehouseId { get; set; }

    /// <summary>Kho đối ứng.</summary>
    public Warehouse? ContraWarehouse { get; set; }

    /// <summary>Loại bút toán: nhập, xuất, điều chỉnh, chuyển kho.</summary>
    public InventoryTxnType Type { get; set; }

    /// <summary>Số lượng thay đổi. Dương = tăng, âm = giảm.</summary>
    public decimal QtyChange { get; set; }

    /// <summary>Số chứng từ nguồn, ví dụ mã phiếu nhập hoặc phiếu xuất.</summary>
    [MaxLength(80)]
    public required string Reference { get; set; }

    /// <summary>Ghi chú.</summary>
    [MaxLength(300)]
    public string? Note { get; set; }
}
