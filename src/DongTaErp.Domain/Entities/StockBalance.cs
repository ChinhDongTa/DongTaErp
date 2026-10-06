
namespace DongTaErp.Domain.Entities;

public class StockBalance:BaseAuditableEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public decimal Qty { get; set; }
    public decimal MinQty { get; set; }
}