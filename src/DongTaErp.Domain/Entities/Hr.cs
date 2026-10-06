using DongTaErp.Domain.Enums;

namespace DongTaErp.Domain.Entities;

public class Department:BaseAuditableEntity
{
    [MaxLength(30)]
    public required string Code { get; set; } 

    [MaxLength(160)]
    public required string Name { get; set; } 

    [MaxLength(300)]
    public string? Note { get; set; } 

    public bool IsActive { get; set; } = true;
    public ICollection<Employee> Employees { get; set; } = [];
}

public class JobPosition:BaseAuditableEntity
{

    [MaxLength(30)]
    public required string Code { get; set; } 

    [MaxLength(160)]
    public required string Name { get; set; } 

    public bool IsActive { get; set; } = true;
    public ICollection<Employee> Employees { get; set; } = [];
}

public class Employee:BaseAuditableEntity
{
    [MaxLength(30)]
    public required string Code { get; set; }

    [MaxLength(160)]
    public required string FullName { get; set; }

    [MaxLength(160)]
    public string? Email { get; set; }

    [MaxLength(40)]
    public string? Phone { get; set; } 

    [MaxLength(40)]
    public required string IdentityNumber { get; set; } 

    [MaxLength(300)]
    public string? Address { get; set; }

    public DateTime? BirthDate { get; set; }
    public DateTime HireDate { get; set; } = DateTime.Today;
    public DateTime? ResignDate { get; set; }

    public Gender Gender { get; set; } = Gender.Other;
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Working;

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public Guid PositionId { get; set; }
    public JobPosition Position { get; set; } = null!;

    public string? UserId { get; set; }
    public AppUser? User { get; set; }

    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = [];
    public virtual ICollection<Payroll> PayrollRecords { get; set; } = [];
}
// ======================== Payroll ========================
/// <summary>
/// Bảng lương
/// </summary>
public class Payroll : BaseAuditableEntity
{
    /// <summary>ID nhân viên</summary>
    public required Guid EmployeeId { get; set; }

    /// <summary>Ngày bắt đầu kỳ lương</summary>
    public DateOnly PeriodStartDate { get; set; }

    /// <summary>Ngày kết thúc kỳ lương</summary>
    public DateOnly PeriodEndDate { get; set; }

    /// <summary>Lương cơ bản</summary>
    public decimal BaseSalary { get; set; }

    /// <summary>Số ngày công</summary>
    public decimal DaysWorked { get; set; }

    /// <summary>Số giờ làm thêm</summary>
    public decimal? OvertimeHours { get; set; }

    /// <summary>Tiền làm thêm</summary>
    public decimal? OvertimeAmount { get; set; }

    /// <summary>Tiền thưởng / phụ cấp</summary>
    public decimal? BonusAmount { get; set; }

    /// <summary>Các khoản khấu trừ</summary>
    public decimal? Deductions { get; set; }

    /// <summary>Chi tiết khấu trừ</summary>
    [MaxLength(1000)]
    public string? DeductionDetails { get; set; }

    /// <summary>Thực lãnh</summary>
    public decimal NetAmount { get; set; }

    /// <summary>Trạng thái thanh toán</summary>
    public PayrollStatus PaymentStatus { get; set; } = PayrollStatus.Pending;

    /// <summary>Ngày thanh toán</summary>
    public DateTimeOffset? PaymentDate { get; set; }

    /// <summary>Phương thức thanh toán</summary>
    [MaxLength(50)]
    public string? PaymentMethod { get; set; }

    /// <summary>Mã tham chiếu / giao dịch</summary>
    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }

    /// <summary>Ghi chú</summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation
    public virtual Employee? Employee { get; set; }
}

public class LeaveRequest:BaseAuditableEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public LeaveType Type { get; set; } = LeaveType.Annual;
    public DateTime FromDate { get; set; } = DateTime.Today;
    public DateTime ToDate { get; set; } = DateTime.Today;
    public decimal Days { get; set; }

    [MaxLength(400)]
    public required string Reason { get; set; } 

    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

    [MaxLength(400)]
    public string? ReviewNote { get; set; } 

    public DateTime? ReviewedAt { get; set; }
}