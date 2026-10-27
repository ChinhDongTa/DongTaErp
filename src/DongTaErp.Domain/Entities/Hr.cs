using DongTaErp.Domain.Enums;

namespace DongTaErp.Domain.Entities;

/// <summary>
/// Phòng ban.
/// </summary>
public class Department : BaseAuditableEntity
{
    /// <summary>Mã phòng ban, duy nhất.</summary>
    [MaxLength(30)]
    public required string Code { get; set; }

    /// <summary>Tên phòng ban.</summary>
    [MaxLength(160)]
    public required string Name { get; set; }

    /// <summary>Ghi chú.</summary>
    [MaxLength(300)]
    public string? Note { get; set; }

    /// <summary>Còn sử dụng hay không.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Nhân viên thuộc phòng.</summary>
    public ICollection<Employee> Employees { get; set; } = [];
}

/// <summary>
/// Chức danh / vị trí công việc.
/// </summary>
public class JobPosition : BaseAuditableEntity
{
    /// <summary>Mã chức danh, duy nhất.</summary>
    [MaxLength(30)]
    public required string Code { get; set; }

    /// <summary>Tên chức danh.</summary>
    [MaxLength(160)]
    public required string Name { get; set; }

    /// <summary>Còn sử dụng hay không.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Nhân viên giữ chức danh này.</summary>
    public ICollection<Employee> Employees { get; set; } = [];
}

/// <summary>
/// Nhân viên. Hồ sơ nhân sự, tách khỏi tài khoản đăng nhập AppUser.
/// </summary>
public class Employee : BaseAuditableEntity
{
    /// <summary>Mã nhân viên, duy nhất.</summary>
    [MaxLength(30)]
    public required string Code { get; set; }

    /// <summary>Họ tên.</summary>
    [MaxLength(160)]
    public required string FullName { get; set; }

    /// <summary>Email.</summary>
    [MaxLength(160)]
    public string? Email { get; set; }

    /// <summary>Số điện thoại.</summary>
    [MaxLength(40)]
    public string? Phone { get; set; }

    /// <summary>Số CCCD / hộ chiếu.</summary>
    [MaxLength(40)]
    public required string IdentityNumber { get; set; }

    /// <summary>Địa chỉ.</summary>
    [MaxLength(300)]
    public string? Address { get; set; }

    /// <summary>Ngày sinh.</summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>Ngày vào làm.</summary>
    public DateTime HireDate { get; set; } = DateTime.Today;

    /// <summary>Ngày nghỉ việc. null nếu còn làm.</summary>
    public DateTime? ResignDate { get; set; }

    /// <summary>Giới tính.</summary>
    public Gender Gender { get; set; } = Gender.Other;

    /// <summary>Trạng thái: thử việc, đang làm, nghỉ phép, đã nghỉ.</summary>
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Working;

    /// <summary>Phòng ban.</summary>
    public Guid DepartmentId { get; set; }

    /// <summary>Phòng ban.</summary>
    public Department Department { get; set; } = null!;

    /// <summary>Chức danh.</summary>
    public Guid PositionId { get; set; }

    /// <summary>Chức danh.</summary>
    public JobPosition Position { get; set; } = null!;

    /// <summary>Tài khoản đăng nhập, nếu nhân viên được cấp quyền vào hệ thống.</summary>
    public string? UserId { get; set; }

    /// <summary>Tài khoản đăng nhập.</summary>
    public AppUser? User { get; set; }

    /// <summary>Các đơn nghỉ phép.</summary>
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = [];

    /// <summary>Các kỳ lương.</summary>
    public virtual ICollection<Payroll> PayrollRecords { get; set; } = [];

    /// <summary>Các khoản lương hợp đồng.</summary>
    public virtual ICollection<EmployeeCompensation> Compensations { get; set; } = [];
}

/// <summary>
/// Phiếu lương một kỳ của một nhân viên. Chỉ giữ tổng và thông tin trả lương.
/// </summary>
public class Payroll : BaseAuditableEntity
{
    /// <summary>Nhân viên.</summary>
    public required Guid EmployeeId { get; set; }

    /// <summary>Nhân viên.</summary>
    public virtual Employee? Employee { get; set; }

    /// <summary>Ngày bắt đầu kỳ lương.</summary>
    public DateOnly PeriodStartDate { get; set; }

    /// <summary>Ngày kết thúc kỳ lương.</summary>
    public DateOnly PeriodEndDate { get; set; }

    /// <summary>Tổng thu nhập.</summary>
    public decimal GrossAmount { get; set; }

    /// <summary>Tổng khấu trừ.</summary>
    public decimal DeductionAmount { get; set; }

    /// <summary>Thực lãnh.</summary>
    public decimal NetAmount { get; set; }

    /// <summary>Trạng thái trả lương.</summary>
    public PayrollStatus PaymentStatus { get; set; } = PayrollStatus.Pending;

    /// <summary>Ngày đã trả lương.</summary>
    public DateTimeOffset? PaymentDate { get; set; }

    /// <summary>Hình thức trả lương.</summary>
    [MaxLength(50)]
    public string? PaymentMethod { get; set; }

    /// <summary>Mã giao dịch hoặc tham chiếu.</summary>
    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }

    /// <summary>Ghi chú.</summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>Các khoản lương của kỳ.</summary>
    public ICollection<PayrollLine> Lines { get; set; } = [];
}

/// <summary>
/// Danh mục khoản lương: lương cơ bản, phụ cấp, làm thêm, BHXH, thuế.
/// </summary>
public class SalaryComponent : BaseAuditableEntity
{
    /// <summary>Mã khoản lương, duy nhất.</summary>
    [MaxLength(30)]
    public required string Code { get; set; }

    /// <summary>Tên khoản lương.</summary>
    [MaxLength(160)]
    public required string Name { get; set; }

    /// <summary>Thu nhập hoặc khấu trừ.</summary>
    public SalaryComponentType Type { get; set; }

    /// <summary>Khoản cố định theo hợp đồng hay khoản phát sinh kỳ.</summary>
    public bool IsFixed { get; set; }

    /// <summary>Có tính thuế thu nhập không.</summary>
    public bool IsTaxable { get; set; }

    /// <summary>Còn sử dụng hay không.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Ghi chú.</summary>
    [MaxLength(300)]
    public string? Note { get; set; }
}

/// <summary>
/// Khoản lương cố định của nhân viên theo thời gian hiệu lực.
/// </summary>
public class EmployeeCompensation : BaseAuditableEntity
{
    /// <summary>Nhân viên.</summary>
    public Guid EmployeeId { get; set; }

    /// <summary>Nhân viên.</summary>
    public Employee Employee { get; set; } = null!;

    /// <summary>Khoản lương.</summary>
    public Guid SalaryComponentId { get; set; }

    /// <summary>Khoản lương.</summary>
    public SalaryComponent SalaryComponent { get; set; } = null!;

    /// <summary>Số tiền hợp đồng.</summary>
    public decimal Amount { get; set; }

    /// <summary>Ngày bắt đầu hiệu lực.</summary>
    public DateOnly EffectiveFrom { get; set; }

    /// <summary>Ngày hết hiệu lực. null nếu còn hiệu lực.</summary>
    public DateOnly? EffectiveTo { get; set; }

    /// <summary>Ghi chú.</summary>
    [MaxLength(300)]
    public string? Note { get; set; }
}

/// <summary>
/// Một dòng trên phiếu lương. Snapshot khoản lương tại thời điểm lập kỳ.
/// </summary>
public class PayrollLine : BaseAuditableEntity
{
    /// <summary>Phiếu lương.</summary>
    public Guid PayrollId { get; set; }

    /// <summary>Phiếu lương.</summary>
    public Payroll Payroll { get; set; } = null!;

    /// <summary>Khoản lương.</summary>
    public Guid SalaryComponentId { get; set; }

    /// <summary>Khoản lương.</summary>
    public SalaryComponent SalaryComponent { get; set; } = null!;

    /// <summary>Số lượng: ngày công, giờ làm thêm. Mặc định 1.</summary>
    public decimal Quantity { get; set; } = 1;

    /// <summary>Đơn giá.</summary>
    public decimal Rate { get; set; }

    /// <summary>Thành tiền.</summary>
    public decimal Amount { get; set; }

    /// <summary>Ghi chú.</summary>
    [MaxLength(300)]
    public string? Note { get; set; }
}

/// <summary>
/// Đơn nghỉ phép. Duyệt hoặc từ chối không ảnh hưởng kho hay công nợ.
/// </summary>
public class LeaveRequest : BaseAuditableEntity
{
    /// <summary>Nhân viên nghỉ.</summary>
    public Guid EmployeeId { get; set; }

    /// <summary>Nhân viên.</summary>
    public Employee Employee { get; set; } = null!;

    /// <summary>Loại nghỉ: phép năm, ốm, không lương, khác.</summary>
    public LeaveType Type { get; set; } = LeaveType.Annual;

    /// <summary>Ngày bắt đầu.</summary>
    public DateTime FromDate { get; set; } = DateTime.Today;

    /// <summary>Ngày kết thúc.</summary>
    public DateTime ToDate { get; set; } = DateTime.Today;

    /// <summary>Số ngày nghỉ.</summary>
    public decimal Days { get; set; }

    /// <summary>Lý do.</summary>
    [MaxLength(400)]
    public required string Reason { get; set; }

    /// <summary>Chờ duyệt, đã duyệt, từ chối, hoặc đã hủy.</summary>
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

    /// <summary>Ghi chú của người duyệt.</summary>
    [MaxLength(400)]
    public string? ReviewNote { get; set; }

    /// <summary>Thời điểm duyệt hoặc từ chối.</summary>
    public DateTime? ReviewedAt { get; set; }
}
