namespace DongTaErp.Application.Services.EmployeeCompensations;

public class CreateEmployeeCompensationValidator : AbstractValidator<CreateEmployeeCompensationDto>
{
    public CreateEmployeeCompensationValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.SalaryComponentId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.EffectiveTo).GreaterThanOrEqualTo(x => x.EffectiveFrom).When(x => x.EffectiveTo.HasValue);
        RuleFor(x => x.Note).MaximumLength(300);
    }
}

public class UpdateEmployeeCompensationValidator : AbstractValidator<UpdateEmployeeCompensationDto>
{
    public UpdateEmployeeCompensationValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).When(x => x.Amount.HasValue);
        RuleFor(x => x.Note).MaximumLength(300);
    }
}
