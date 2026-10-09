namespace DongTaErp.Application.Services.CompanyProfiles;

public class CreateCompanyProfileValidator : AbstractValidator<CreateCompanyProfileDto>
{
    public CreateCompanyProfileValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.TaxCode)
            .MaximumLength(40);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .MaximumLength(40);

        RuleFor(x => x.Email)
            .MaximumLength(160)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .MaximumLength(20);
    }
}

public class UpdateCompanyProfileValidator : AbstractValidator<UpdateCompanyProfileDto>
{
    public UpdateCompanyProfileValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MaximumLength(200)
            .When(x => x.Name != null);

        RuleFor(x => x.TaxCode)
            .MaximumLength(40)
            .When(x => x.TaxCode != null);

        RuleFor(x => x.Phone)
            .MaximumLength(40)
            .When(x => x.Phone != null);

        RuleFor(x => x.Email)
            .MaximumLength(160)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Address)
            .MaximumLength(300)
            .When(x => x.Address != null);

        RuleFor(x => x.Currency)
            .MaximumLength(20)
            .When(x => x.Currency != null);
    }
}