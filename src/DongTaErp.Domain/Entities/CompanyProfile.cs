
namespace DongTaErp.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class CompanyProfile:BaseAuditableEntity
{

    [MaxLength(200)]
    public string Name { get; set; } = "Công ty ERP Lite";

    [MaxLength(40)]
    public string TaxCode { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Currency { get; set; } = "VND";
}
