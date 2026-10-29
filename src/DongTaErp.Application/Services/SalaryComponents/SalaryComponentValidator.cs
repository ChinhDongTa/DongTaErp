namespace DongTaErp.Application.Services.SalaryComponents;

public class CreateSalaryComponentValidator : AbstractValidator<CreateSalaryComponentDto>
{
    public CreateSalaryComponentValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Note).MaximumLength(300);
    }
}

public class UpdateSalaryComponentValidator : AbstractValidator<UpdateSalaryComponentDto>
{
    public UpdateSalaryComponentValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).MaximumLength(30).When(x => x.Code != null);
        RuleFor(x => x.Name).MaximumLength(160).When(x => x.Name != null);
        RuleFor(x => x.Type).IsInEnum().When(x => x.Type.HasValue);
        RuleFor(x => x.Note).MaximumLength(300);
    }
}
