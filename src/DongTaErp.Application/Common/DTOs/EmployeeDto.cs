using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Common.DTOs;

public class EmployeeDto : BaseDto
{
    public string Code { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string IdentityNumber { get; set; } = null!;

    public string? Address { get; set; }

    public DateTime? BirthDate { get; set; }

    public DateTime HireDate { get; set; }

    public DateTime? ResignDate { get; set; }

    /// <summary>
    /// Giới tính
    /// </summary>
    public string Gender { get; set; } = null!;

    /// <summary>
    /// Trạng thái nhân viên
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// Phòng ban
    /// </summary>
    public string DepartmentName { get; set; } = null!;

    /// <summary>
    /// Chức vụ
    /// </summary>
    public string PositionName { get; set; } = null!;

    /// <summary>
    /// Tài khoản đăng nhập
    /// </summary>
    public string? UserName { get; set; }
}

public class CreateEmployeeDto
{
    /// <summary>
    /// Mã nhân viên.
    /// Bắt buộc.
    /// Tối đa 30 ký tự.
    /// Không được trùng.
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Họ và tên.
    /// Bắt buộc.
    /// Tối đa 160 ký tự.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// Email.
    /// Không bắt buộc.
    /// Tối đa 160 ký tự.
    /// Phải đúng định dạng email.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Số điện thoại.
    /// Không bắt buộc.
    /// Tối đa 40 ký tự.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// CCCD/CMND.
    /// Bắt buộc.
    /// Tối đa 40 ký tự.
    /// </summary>
    public required string IdentityNumber { get; set; }

    /// <summary>
    /// Địa chỉ.
    /// Không bắt buộc.
    /// Tối đa 300 ký tự.
    /// </summary>
    public string? Address { get; set; }

    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// Ngày vào làm.
    /// Bắt buộc.
    /// </summary>
    public DateTime HireDate { get; set; } = DateTime.Today;

    public DateTime? ResignDate { get; set; }

    /// <summary>
    /// Giới tính.
    /// Bắt buộc.
    /// Giá trị phải thuộc enum Gender.
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// Trạng thái nhân viên.
    /// Bắt buộc.
    /// Giá trị phải thuộc enum EmployeeStatus.
    /// </summary>
    public EmployeeStatus Status { get; set; }

    /// <summary>
    /// Phòng ban.
    /// Bắt buộc.
    /// Phải tồn tại trong hệ thống.
    /// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// Chức vụ.
    /// Bắt buộc.
    /// Phải tồn tại trong hệ thống.
    /// </summary>
    public Guid PositionId { get; set; }

    /// <summary>
    /// Tài khoản người dùng.
    /// Không bắt buộc.
    /// </summary>
    public string? UserId { get; set; }
}

public class UpdateEmployeeDto
{
    /// <summary>
    /// Id nhân viên.
    /// Bắt buộc.
    /// </summary>
    public Guid Id { get; set; }

    public string? Code { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? IdentityNumber { get; set; }

    public string? Address { get; set; }

    public DateTime? BirthDate { get; set; }

    public DateTime? HireDate { get; set; }

    public DateTime? ResignDate { get; set; }

    public Gender? Gender { get; set; }

    public EmployeeStatus? Status { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? PositionId { get; set; }

    public string? UserId { get; set; }
}