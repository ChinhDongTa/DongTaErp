
namespace DongTaErp.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class CompanyProfile:BaseAuditableEntity
{

    [MaxLength(200)]
    public required string Name { get; set; } = "Công ty ERP Lite";

    [MaxLength(40)]
    public string? TaxCode { get; set; }

    [MaxLength(40)]
    public required string Phone { get; set; }

    [MaxLength(160)]
    public string? Email { get; set; }

    [MaxLength(300)]
    public required string Address { get; set; }

    [MaxLength(20)]
    public required string Currency { get; set; } = "VND";
}