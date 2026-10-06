namespace DongTaErp.Application.Common.DTOs;

using DongTaErp.Domain.Enums;

public class WarehouseDto : BaseDto
{
    /// <summary>
    /// Mã kho.
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Tên kho.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Loại kho.
    /// </summary>
    public WarehouseType Type { get; set; }
    public string TypeName=>Type.ToDisplayName();

    /// <summary>
    /// Mã quốc gia (ISO 3166-1 alpha-2).
    /// </summary>
    public string CountryCode { get; set; } = null!;

    /// <summary>
    /// Thành phố.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Địa chỉ kho.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Múi giờ.
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Kho mặc định.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Trạng thái hoạt động.
    /// </summary>
    public bool IsActive { get; set; }
}

public class CreateWarehouseDto
{
    /// <summary>
    /// Mã kho.
    /// Bắt buộc.
    /// Tối đa 30 ký tự.
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Tên kho.
    /// Bắt buộc.
    /// Tối đa 160 ký tự.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Loại kho.
    /// Không bắt buộc.
    /// Mặc định: Branch.
    /// </summary>
    public WarehouseType Type { get; set; } = WarehouseType.Branch;

    /// <summary>
    /// Mã quốc gia.
    /// Bắt buộc.
    /// Tối đa 2 ký tự.
    /// Mặc định: VN.
    /// </summary>
    public string CountryCode { get; set; } = "VN";

    /// <summary>
    /// Thành phố.
    /// Không bắt buộc.
    /// Tối đa 80 ký tự.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Địa chỉ kho.
    /// Không bắt buộc.
    /// Tối đa 300 ký tự.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Múi giờ.
    /// Không bắt buộc.
    /// Tối đa 64 ký tự.
    /// Mặc định: Asia/Ho_Chi_Minh.
    /// </summary>
    public string? TimeZone { get; set; } = "Asia/Ho_Chi_Minh";

    /// <summary>
    /// Đánh dấu là kho mặc định.
    /// Không bắt buộc.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Trạng thái hoạt động.
    /// Không bắt buộc.
    /// Mặc định: true.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

public class UpdateWarehouseDto
{
    /// <summary>
    /// Id kho.
    /// Bắt buộc.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Mã kho.
    /// Không bắt buộc khi cập nhật.
    /// Nếu có thì tối đa 30 ký tự.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Tên kho.
    /// Không bắt buộc khi cập nhật.
    /// Nếu có thì tối đa 160 ký tự.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Loại kho.
    /// Không bắt buộc khi cập nhật.
    /// </summary>
    public WarehouseType? Type { get; set; }

    /// <summary>
    /// Mã quốc gia.
    /// Không bắt buộc khi cập nhật.
    /// Nếu có thì tối đa 2 ký tự.
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// Thành phố.
    /// Không bắt buộc khi cập nhật.
    /// Nếu có thì tối đa 80 ký tự.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Địa chỉ kho.
    /// Không bắt buộc khi cập nhật.
    /// Nếu có thì tối đa 300 ký tự.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Múi giờ.
    /// Không bắt buộc khi cập nhật.
    /// Nếu có thì tối đa 64 ký tự.
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Đánh dấu là kho mặc định.
    /// Không bắt buộc khi cập nhật.
    /// </summary>
    public bool? IsDefault { get; set; }

    /// <summary>
    /// Trạng thái hoạt động.
    /// Không bắt buộc khi cập nhật.
    /// </summary>
    public bool? IsActive { get; set; }
}