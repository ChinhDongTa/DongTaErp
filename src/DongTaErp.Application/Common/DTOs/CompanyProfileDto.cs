namespace DongTaErp.Application.Common.DTOs;

public class CompanyProfileDto : BaseDto
{
    public string Name { get; set; } = null!;

    public string? TaxCode { get; set; }

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string Address { get; set; } = null!;

    public string Currency { get; set; } = null!;
}

public class CreateCompanyProfileDto
{
    /// <summary>
    /// Tên công ty.
    /// Bắt buộc.
    /// Tối đa 200 ký tự.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Mã số thuế.
    /// Không bắt buộc.
    /// Tối đa 40 ký tự.
    /// </summary>
    public string? TaxCode { get; set; }

    /// <summary>
    /// Số điện thoại.
    /// Bắt buộc.
    /// Tối đa 40 ký tự.
    /// </summary>
    public required string Phone { get; set; }

    /// <summary>
    /// Email.
    /// Không bắt buộc.
    /// Tối đa 160 ký tự.
    /// Nếu nhập phải đúng định dạng email.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Địa chỉ công ty.
    /// Bắt buộc.
    /// Tối đa 300 ký tự.
    /// </summary>
    public required string Address { get; set; }

    /// <summary>
    /// Đơn vị tiền tệ mặc định.
    /// Bắt buộc.
    /// Tối đa 20 ký tự.
    /// Ví dụ: VND, USD, EUR.
    /// </summary>
    public required string Currency { get; set; } = "VND";
}

public class UpdateCompanyProfileDto
{
    /// <summary>
    /// Id hồ sơ công ty.
    /// Bắt buộc.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Tên công ty.
    /// Không bắt buộc khi cập nhật.
    /// Nếu nhập phải tối đa 200 ký tự.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Mã số thuế.
    /// Không bắt buộc khi cập nhật.
    /// Nếu nhập phải tối đa 40 ký tự.
    /// </summary>
    public string? TaxCode { get; set; }

    /// <summary>
    /// Số điện thoại.
    /// Không bắt buộc khi cập nhật.
    /// Nếu nhập phải tối đa 40 ký tự.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Email.
    /// Không bắt buộc khi cập nhật.
    /// Nếu nhập phải tối đa 160 ký tự.
    /// Phải đúng định dạng email.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Địa chỉ công ty.
    /// Không bắt buộc khi cập nhật.
    /// Nếu nhập phải tối đa 300 ký tự.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Đơn vị tiền tệ mặc định.
    /// Không bắt buộc khi cập nhật.
    /// Nếu nhập phải tối đa 20 ký tự.
    /// Ví dụ: VND, USD, EUR.
    /// </summary>
    public string? Currency { get; set; }
}