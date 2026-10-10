using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Common.DTOs;

public class PartnerDto : BaseDto
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;
    public PartnerType Type { get; set; }
    public string TypeName => Type.ToDisplayName();

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? TaxCode { get; set; }

    public string? Address { get; set; }

    public bool IsActive { get; set; }
}

public class CreatePartnerDto
{
    /// <summary>
    /// Mã đối tác.
    /// Bắt buộc nhập.
    /// Tối đa 40 ký tự.
    /// Không được trùng lặp trong hệ thống.
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Tên đối tác.
    /// Bắt buộc nhập.
    /// Tối đa 200 ký tự.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Loại đối tác.
    /// Giá trị hợp lệ: Customer, Supplier hoặc các giá trị được định nghĩa trong PartnerType.
    /// Mặc định là Customer.
    /// </summary>
    public PartnerType Type { get; set; } = PartnerType.Customer;

    /// <summary>
    /// Số điện thoại.
    /// Không bắt buộc.
    /// Tối đa 40 ký tự.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Email.
    /// Không bắt buộc.
    /// Tối đa 160 ký tự.
    /// Đúng định dạng email nếu được nhập.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Mã số thuế.
    /// Không bắt buộc.
    /// Tối đa 40 ký tự.
    /// </summary>
    public string? TaxCode { get; set; }

    /// <summary>
    /// Địa chỉ.
    /// Không bắt buộc.
    /// Tối đa 300 ký tự.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Trạng thái hoạt động.
    /// Mặc định là true.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

public class UpdatePartnerDto
{
    /// <summary>
    /// Id đối tác.
    /// Bắt buộc.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Mã đối tác.
    /// Không bắt buộc khi cập nhật.
    /// Nếu nhập phải tối đa 40 ký tự.
    /// Không được trùng lặp trong hệ thống.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Tên đối tác.
    /// Không bắt buộc khi cập nhật.
    /// Nếu nhập phải tối đa 200 ký tự.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Loại đối tác.
    /// Không bắt buộc khi cập nhật.
    /// Giá trị phải thuộc enum PartnerType.
    /// </summary>
    public PartnerType? Type { get; set; }

    /// <summary>
    /// Số điện thoại.
    /// Không bắt buộc.
    /// Nếu nhập phải tối đa 40 ký tự.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Email.
    /// Không bắt buộc.
    /// Nếu nhập phải tối đa 160 ký tự.
    /// Phải đúng định dạng email.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Mã số thuế.
    /// Không bắt buộc.
    /// Nếu nhập phải tối đa 40 ký tự.
    /// </summary>
    public string? TaxCode { get; set; }

    /// <summary>
    /// Địa chỉ.
    /// Không bắt buộc.
    /// Nếu nhập phải tối đa 300 ký tự.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Trạng thái hoạt động.
    /// Không bắt buộc khi cập nhật.
    /// </summary>
    public bool? IsActive { get; set; }
}
