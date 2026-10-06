namespace DongTaErp.Application.Common.DTOs;

public class JobPositionDto : BaseDto
{
    /// <summary>
    /// Mã chức vụ
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Tên chức vụ
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool IsActive { get; set; }
}

public class CreateJobPositionDto
{
    /// <summary>
    /// Mã chức vụ.
    /// Bắt buộc.
    /// Tối đa 30 ký tự.
    /// Không được trùng trong hệ thống.
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Tên chức vụ.
    /// Bắt buộc.
    /// Tối đa 160 ký tự.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Trạng thái hoạt động.
    /// Mặc định là true.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

public class UpdateJobPositionDto
{
    /// <summary>
    /// Id chức vụ.
    /// Bắt buộc.
    /// </summary>
    public Guid Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public bool? IsActive { get; set; }
}