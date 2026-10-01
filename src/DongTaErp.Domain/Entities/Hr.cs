using DongTaErp.Domain.Enums;

namespace DongTaErp.Domain.Entities;

public class Department:BaseAuditableEntity
{
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Note { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

public class JobPosition:BaseAuditableEntity
{

    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

public class Employee:BaseAuditableEntity
{
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(160)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(40)]
    public string IdentityNumber { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Address { get; set; } = string.Empty;

    public DateTime? BirthDate { get; set; }
    public DateTime HireDate { get; set; } = DateTime.Today;
    public DateTime? ResignDate { get; set; }

    public Gender Gender { get; set; } = Gender.Other;
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Working;

    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public int PositionId { get; set; }
    public JobPosition Position { get; set; } = null!;

    public string? UserId { get; set; }
    public AppUser? User { get; set; }

    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
}

public class LeaveRequest:BaseAuditableEntity
{
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public LeaveType Type { get; set; } = LeaveType.Annual;
    public DateTime FromDate { get; set; } = DateTime.Today;
    public DateTime ToDate { get; set; } = DateTime.Today;
    public decimal Days { get; set; }

    [MaxLength(400)]
    public string Reason { get; set; } = string.Empty;

    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

    [MaxLength(400)]
    public string ReviewNote { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
}