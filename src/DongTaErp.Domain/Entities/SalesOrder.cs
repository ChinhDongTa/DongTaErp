
namespace DongTaErp.Domain.Entities;

using DongTaErp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class SalesOrder:BaseAuditableEntity
{

    [MaxLength(30)]
    public string Number { get; set; } = string.Empty;

    public int PartnerId { get; set; }
    public Partner Partner { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.Today;
    public SalesOrderStatus Status { get; set; } = SalesOrderStatus.Draft;

    [MaxLength(400)]
    public string Notes { get; set; } = string.Empty;

    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; } = 0.08m;
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }

    public ICollection<SalesOrderLine> Lines { get; set; } = new List<SalesOrderLine>();
}
