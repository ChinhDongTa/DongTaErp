namespace DongTaErp.Application.Services.Payrolls;

public class PayrollValidator : AbstractValidator<CreatePayrollDto>
{
    public PayrollValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.PeriodEndDate).GreaterThanOrEqualTo(x => x.PeriodStartDate);
        RuleFor(x => x.PaymentMethod).MaximumLength(50);
        RuleFor(x => x.ReferenceNumber).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(500);
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.SalaryComponentId).NotEmpty();
            line.RuleFor(x => x.Quantity).GreaterThan(0);
            line.RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        });
    }
}

public class UpdatePayrollValidator : AbstractValidator<UpdatePayrollDto>
{
    public UpdatePayrollValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PaymentMethod).MaximumLength(50);
        RuleFor(x => x.ReferenceNumber).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
