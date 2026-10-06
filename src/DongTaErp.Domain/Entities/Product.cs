namespace DongTaErp.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class Product : BaseAuditableEntity
{
    [MaxLength(40)]
    public required string Sku { get; set; } 

    [MaxLength(200)]
    public required string Name { get; set; } 

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    [MaxLength(20)]
    public required string Unit { get; set; } = "Cái";

    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal MinStock { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<SalesOrderLine> SalesLines { get; set; } = [];
    public ICollection<InventoryTxn> InventoryTxns { get; set; } = [];
    public ICollection<StockBalance> StockBalances { get; set; } = [];
}
