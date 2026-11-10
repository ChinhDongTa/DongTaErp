namespace DongTaErp.Application.Services.Departments;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(160);

        RuleFor(x => x.Note)
            .MaximumLength(300);
    }
}

public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentDto>
{
    public UpdateDepartmentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Code)
            .MaximumLength(30)
            .When(x => x.Code != null);

        RuleFor(x => x.Name)
            .MaximumLength(160)
            .When(x => x.Name != null);

        RuleFor(x => x.Note)
            .MaximumLength(300)
            .When(x => x.Note != null);
    }
}