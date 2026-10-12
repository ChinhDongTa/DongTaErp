namespace DongTaErp.Application.Services.Payrolls;

public class CreatePayrollValidator : AbstractValidator<CreatePayrollDto>
{
    public CreatePayrollValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.PeriodStartDate).NotEmpty();
        RuleFor(x => x.PeriodEndDate).GreaterThanOrEqualTo(x => x.PeriodStartDate);
        RuleFor(x => x.BaseSalary).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DaysWorked).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OvertimeHours).GreaterThanOrEqualTo(0).When(x => x.OvertimeHours.HasValue);
        RuleFor(x => x.DeductionDetails).MaximumLength(1000);
        RuleFor(x => x.PaymentMethod).MaximumLength(50);
        RuleFor(x => x.ReferenceNumber).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(500);
        RuleFor(x => x.PaymentStatus).IsInEnum();
    }
}

public class UpdatePayrollValidator : AbstractValidator<UpdatePayrollDto>
{
    public UpdatePayrollValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.BaseSalary).GreaterThanOrEqualTo(0).When(x => x.BaseSalary.HasValue);
        RuleFor(x => x.DaysWorked).GreaterThanOrEqualTo(0).When(x => x.DaysWorked.HasValue);
        RuleFor(x => x.DeductionDetails).MaximumLength(1000).When(x => x.DeductionDetails != null);
        RuleFor(x => x.PaymentMethod).MaximumLength(50).When(x => x.PaymentMethod != null);
        RuleFor(x => x.ReferenceNumber).MaximumLength(100).When(x => x.ReferenceNumber != null);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes != null);
        RuleFor(x => x.PaymentStatus).IsInEnum().When(x => x.PaymentStatus.HasValue);
    }
}
