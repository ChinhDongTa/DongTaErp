namespace DongTaErp.Domain.Entities;

using DongTaErp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class SalesOrder:BaseAuditableEntity
{
    [MaxLength(30)]
    public required string Number { get; set; }

    public Guid PartnerId { get; set; }
    public Partner Partner { get; set; } = null!;
    /// <summary>Kho mặc định khi thêm dòng. Nguồn xuất thật nằm trên từng SalesOrderLine.</summary>
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public DateTime OrderDate { get; set; } = DateTime.Today;
    public SalesOrderStatus Status { get; set; } = SalesOrderStatus.Draft;

    [MaxLength(400)]
    public string? Notes { get; set; } 

    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; } = 0.08m;
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public ICollection<SalesOrderLine> Lines { get; set; } = [];
}
