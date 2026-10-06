using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Common.DTOs;

public class LeaveRequestDto : BaseDto
{
    /// <summary>
    /// Nhân viên
    /// </summary>
    public string EmployeeName { get; set; } = null!;

    public LeaveType Type { get; set; }

    /// <summary>
    /// Loại nghỉ phép
    /// </summary>
    public string TypeName => Type.ToDisplayName();

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public decimal Days { get; set; }

    public string Reason { get; set; } = null!;
    public LeaveStatus Status { get; set; }
    /// <summary>
    /// Trạng thái duyệt
    /// </summary>
    public string StatusName => Status.ToDisplayName();

    public string? ReviewNote { get; set; }

    public DateTime? ReviewedAt { get; set; }
}

public class CreateLeaveRequestDto
{
    /// <summary>
    /// Nhân viên.
    /// Bắt buộc.
    /// Phải tồn tại trong hệ thống.
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Loại nghỉ phép.
    /// Bắt buộc.
    /// Giá trị phải thuộc enum LeaveType.
    /// </summary>
    public LeaveType Type { get; set; } = LeaveType.Annual;

    /// <summary>
    /// Từ ngày.
    /// Bắt buộc.
    /// </summary>
    public DateTime FromDate { get; set; }

    /// <summary>
    /// Đến ngày.
    /// Bắt buộc.
    /// Phải lớn hơn hoặc bằng Từ ngày.
    /// </summary>
    public DateTime ToDate { get; set; }

    /// <summary>
    /// Số ngày nghỉ.
    /// Bắt buộc.
    /// Phải lớn hơn 0.
    /// </summary>
    public decimal Days { get; set; }

    /// <summary>
    /// Lý do nghỉ.
    /// Bắt buộc.
    /// Tối đa 400 ký tự.
    /// </summary>
    public required string Reason { get; set; }

    /// <summary>
    /// Trạng thái duyệt.
    /// Giá trị phải thuộc enum LeaveStatus.
    /// </summary>
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

    /// <summary>
    /// Ghi chú phê duyệt.
    /// Không bắt buộc.
    /// Tối đa 400 ký tự.
    /// </summary>
    public string? ReviewNote { get; set; }

    public DateTime? ReviewedAt { get; set; }
}

public class UpdateLeaveRequestDto
{
    /// <summary>
    /// Id đơn nghỉ phép.
    /// Bắt buộc.
    /// </summary>
    public Guid Id { get; set; }

    public Guid? EmployeeId { get; set; }

    public LeaveType? Type { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public decimal? Days { get; set; }

    public string? Reason { get; set; }

    public LeaveStatus? Status { get; set; }

    public string? ReviewNote { get; set; }

    public DateTime? ReviewedAt { get; set; }
}