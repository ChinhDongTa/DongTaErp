namespace DongTaErp.Application.Services.Employees;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(160).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).MaximumLength(40);
        RuleFor(x => x.IdentityNumber).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Address).MaximumLength(300);
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.DepartmentId).NotEmpty();
        RuleFor(x => x.PositionId).NotEmpty();
        RuleFor(x => x.HireDate).NotEmpty();
    }
}

public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeDto>
{
    public UpdateEmployeeValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).MaximumLength(30).When(x => x.Code != null);
        RuleFor(x => x.FullName).MaximumLength(160).When(x => x.FullName != null);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(160).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).MaximumLength(40).When(x => x.Phone != null);
        RuleFor(x => x.IdentityNumber).MaximumLength(40).When(x => x.IdentityNumber != null);
        RuleFor(x => x.Address).MaximumLength(300).When(x => x.Address != null);
        RuleFor(x => x.Gender).IsInEnum().When(x => x.Gender.HasValue);
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
    }
}
