namespace DongTaErp.Domain.Entities;

using DongTaErp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class InventoryTxn:BaseAuditableEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public InventoryTxnType Type { get; set; }
    public decimal QtyChange { get; set; }

    [MaxLength(80)]
    public string Reference { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Note { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}