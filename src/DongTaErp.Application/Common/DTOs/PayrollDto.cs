using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Common.DTOs;

public class PayrollDto : BaseDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;

    public DateOnly PeriodStartDate { get; set; }
    public DateOnly PeriodEndDate { get; set; }

    public decimal BaseSalary { get; set; }
    public decimal DaysWorked { get; set; }
    public decimal? OvertimeHours { get; set; }
    public decimal? OvertimeAmount { get; set; }
    public decimal? BonusAmount { get; set; }
    public decimal? Deductions { get; set; }
    public string? DeductionDetails { get; set; }
    public decimal NetAmount { get; set; }

    public PayrollStatus PaymentStatus { get; set; }
    public string PaymentStatusName => PaymentStatus.ToDisplayName();

    public DateTimeOffset? PaymentDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}

public class CreatePayrollDto
{
    /// <summary>Nhân viên. Bắt buộc. Phải tồn tại.</summary>
    public Guid EmployeeId { get; set; }

    public DateOnly PeriodStartDate { get; set; }
    public DateOnly PeriodEndDate { get; set; }

    /// <summary>Lương cơ bản. Bắt buộc, >= 0.</summary>
    public decimal BaseSalary { get; set; }

    public decimal DaysWorked { get; set; }
    public decimal? OvertimeHours { get; set; }
    public decimal? OvertimeAmount { get; set; }
    public decimal? BonusAmount { get; set; }
    public decimal? Deductions { get; set; }

    /// <summary>Chi tiết khấu trừ. Tối đa 1000 ký tự.</summary>
    public string? DeductionDetails { get; set; }

    /// <summary>Thực lãnh. Nếu không truyền sẽ tự tính.</summary>
    public decimal? NetAmount { get; set; }

    public PayrollStatus PaymentStatus { get; set; } = PayrollStatus.Pending;
    public DateTimeOffset? PaymentDate { get; set; }

    /// <summary>Tối đa 50 ký tự.</summary>
    public string? PaymentMethod { get; set; }

    /// <summary>Tối đa 100 ký tự.</summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>Tối đa 500 ký tự.</summary>
    public string? Notes { get; set; }
}

public class UpdatePayrollDto
{
    public Guid Id { get; set; }

    public Guid? EmployeeId { get; set; }
    public DateOnly? PeriodStartDate { get; set; }
    public DateOnly? PeriodEndDate { get; set; }

    public decimal? BaseSalary { get; set; }
    public decimal? DaysWorked { get; set; }
    public decimal? OvertimeHours { get; set; }
    public decimal? OvertimeAmount { get; set; }
    public decimal? BonusAmount { get; set; }
    public decimal? Deductions { get; set; }
    public string? DeductionDetails { get; set; }
    public decimal? NetAmount { get; set; }

    public PayrollStatus? PaymentStatus { get; set; }
    public DateTimeOffset? PaymentDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}