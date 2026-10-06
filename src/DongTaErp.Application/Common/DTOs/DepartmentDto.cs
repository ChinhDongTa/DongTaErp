namespace DongTaErp.Application.Common.DTOs;

public class DepartmentDto : BaseDto
{
    /// <summary>
    /// Mã phòng ban
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Tên phòng ban
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool IsActive { get; set; }
}

public class CreateDepartmentDto
{
    /// <summary>
    /// Mã phòng ban.
    /// Bắt buộc.
    /// Tối đa 30 ký tự.
    /// Không được trùng trong hệ thống.
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Tên phòng ban.
    /// Bắt buộc.
    /// Tối đa 160 ký tự.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Ghi chú.
    /// Không bắt buộc.
    /// Tối đa 300 ký tự.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Trạng thái hoạt động.
    /// Mặc định là true.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

public class UpdateDepartmentDto
{
    /// <summary>
    /// Id phòng ban.
    /// Bắt buộc.
    /// </summary>
    public Guid Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Note { get; set; }

    public bool? IsActive { get; set; }
}