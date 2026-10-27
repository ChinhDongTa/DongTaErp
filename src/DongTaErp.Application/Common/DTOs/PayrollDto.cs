using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Common.DTOs;

public class PayrollDto : BaseDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public DateOnly PeriodStartDate { get; set; }
    public DateOnly PeriodEndDate { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public decimal NetAmount { get; set; }
    public PayrollStatus PaymentStatus { get; set; }
    public string PaymentStatusName => PaymentStatus.ToDisplayName();
    public DateTimeOffset? PaymentDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public List<PayrollLineDto> Lines { get; set; } = [];
}

public class PayrollLineDto
{
    public Guid Id { get; set; }
    public Guid SalaryComponentId { get; set; }
    public string ComponentName { get; set; } = "";
    public SalaryComponentType Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
}

public class CreatePayrollDto
{
    public Guid EmployeeId { get; set; }
    public DateOnly PeriodStartDate { get; set; }
    public DateOnly PeriodEndDate { get; set; }
    public PayrollStatus PaymentStatus { get; set; } = PayrollStatus.Pending;
    public DateTimeOffset? PaymentDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public List<CreatePayrollLineDto> Lines { get; set; } = [];
}

public class CreatePayrollLineDto
{
    public Guid SalaryComponentId { get; set; }
    public decimal Quantity { get; set; } = 1;
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
}

public class UpdatePayrollDto
{
    public Guid Id { get; set; }
    public Guid? EmployeeId { get; set; }
    public DateOnly? PeriodStartDate { get; set; }
    public DateOnly? PeriodEndDate { get; set; }
    public PayrollStatus? PaymentStatus { get; set; }
    public DateTimeOffset? PaymentDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public List<CreatePayrollLineDto>? Lines { get; set; }
}
