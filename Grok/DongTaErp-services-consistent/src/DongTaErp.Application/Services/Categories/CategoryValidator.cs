namespace DongTaErp.Application.Services.Categories;

public class CreateCategoryValidator:AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Code)            .NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
    }
}

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Code).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Code));
        RuleFor(x => x.Name).MaximumLength(250).When(x => !string.IsNullOrEmpty(x.Name));
    }
}