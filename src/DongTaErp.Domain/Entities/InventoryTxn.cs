namespace DongTaErp.Domain.Entities;

using DongTaErp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class InventoryTxn:BaseAuditableEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public Guid? ContraWarehouseId { get; set; }
    public Warehouse? ContraWarehouse { get; set; }

    public InventoryTxnType Type { get; set; }
    public decimal QtyChange { get; set; }

    [MaxLength(80)]
    public required string Reference { get; set; }

    [MaxLength(300)]
    public string? Note { get; set; } 
}