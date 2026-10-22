namespace DongTaErp.Domain.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Hồ sơ công ty đang vận hành hệ thống. Chỉ nên có một bản ghi.
/// </summary>
public class CompanyProfile : BaseAuditableEntity
{
    /// <summary>Tên công ty.</summary>
    [MaxLength(200)]
    public required string Name { get; set; } = "Công ty ERP Lite";

    /// <summary>Mã số thuế.</summary>
    [MaxLength(40)]
    public string? TaxCode { get; set; }

    /// <summary>Số điện thoại.</summary>
    [MaxLength(40)]
    public required string Phone { get; set; }

    /// <summary>Email.</summary>
    [MaxLength(160)]
    public string? Email { get; set; }

    /// <summary>Địa chỉ.</summary>
    [MaxLength(300)]
    public required string Address { get; set; }

    /// <summary>Tiền tệ mặc định, ví dụ VND.</summary>
    [MaxLength(20)]
    public required string Currency { get; set; } = "VND";
}