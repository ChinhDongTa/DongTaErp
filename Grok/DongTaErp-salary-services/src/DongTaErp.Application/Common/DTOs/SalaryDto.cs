using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Common.DTOs;

public class SalaryComponentDto : BaseDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public SalaryComponentType Type { get; set; }
    public string TypeName => Type.ToDisplayName();
    public bool IsFixed { get; set; }
    public bool IsTaxable { get; set; }
    public bool IsActive { get; set; }
    public string? Note { get; set; }
}

public class CreateSalaryComponentDto
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public SalaryComponentType Type { get; set; }
    public bool IsFixed { get; set; }
    public bool IsTaxable { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Note { get; set; }
}

public class UpdateSalaryComponentDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public SalaryComponentType? Type { get; set; }
    public bool? IsFixed { get; set; }
    public bool? IsTaxable { get; set; }
    public bool? IsActive { get; set; }
    public string? Note { get; set; }
}

public class EmployeeCompensationDto : BaseDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = "";
    public Guid SalaryComponentId { get; set; }
    public string ComponentCode { get; set; } = "";
    public string ComponentName { get; set; } = "";
    public SalaryComponentType Type { get; set; }
    public string TypeName => Type.ToDisplayName();
    public decimal Amount { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
    public string? Note { get; set; }
}

public class CreateEmployeeCompensationDto
{
    public Guid EmployeeId { get; set; }
    public Guid SalaryComponentId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public string? Note { get; set; }
}

public class UpdateEmployeeCompensationDto
{
    public Guid Id { get; set; }
    public Guid? EmployeeId { get; set; }
    public Guid? SalaryComponentId { get; set; }
    public decimal? Amount { get; set; }
    public DateOnly? EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public string? Note { get; set; }
}
