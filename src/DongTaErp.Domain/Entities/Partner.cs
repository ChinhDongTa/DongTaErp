
namespace DongTaErp.Domain.Entities;

using DongTaErp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class Partner:BaseAuditableEntity
{

    [MaxLength(40)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public PartnerType Type { get; set; } = PartnerType.Customer;

    [MaxLength(40)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(40)]
    public string TaxCode { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Address { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
}
