namespace DongTaErp.Application.Services.Disbursements;

public class CreateDisbursementValidator : AbstractValidator<CreateDisbursementDto>
{
    public CreateDisbursementValidator()
    {
        RuleFor(x => x.Number).NotEmpty().MaximumLength(30);
        RuleFor(x => x.PartnerId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.ReferenceNumber).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(400);
        RuleForEach(x => x.Allocations).ChildRules(line =>
        {
            line.RuleFor(x => x.VendorBillId).NotEmpty();
            line.RuleFor(x => x.Amount).GreaterThan(0);
        });
    }
}

public class UpdateDisbursementValidator : AbstractValidator<UpdateDisbursementDto>
{
    public UpdateDisbursementValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Number).MaximumLength(30).When(x => x.Number != null);
        RuleFor(x => x.Amount).GreaterThan(0).When(x => x.Amount != null);
        RuleFor(x => x.ReferenceNumber).MaximumLength(100).When(x => x.ReferenceNumber != null);
        RuleFor(x => x.Notes).MaximumLength(400).When(x => x.Notes != null);
    }
}
