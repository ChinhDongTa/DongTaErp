
namespace DongTaErp.Domain.Entities;

using DongTaErp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class Partner:BaseAuditableEntity
{

    [MaxLength(40)]
    public required string Code { get; set; }

    [MaxLength(200)]
    public required string Name { get; set; }

    public PartnerType Type { get; set; } = PartnerType.Customer;

    [MaxLength(40)]
    public string? Phone { get; set; }

    [MaxLength(160)]
    public string? Email { get; set; }

    [MaxLength(40)]
    public string? TaxCode { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<SalesOrder> SalesOrders { get; set; } = [];
}
