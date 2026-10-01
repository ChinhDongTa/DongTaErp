
namespace DongTaErp.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class Product : BaseAuditableEntity
{

    [MaxLength(40)]
    public string Sku { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Unit { get; set; } = "Cái";

    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal StockQty { get; set; }
    public decimal MinStock { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SalesOrderLine> SalesLines { get; set; } = new List<SalesOrderLine>();
    public ICollection<InventoryTxn> InventoryTxns { get; set; } = new List<InventoryTxn>();
}
