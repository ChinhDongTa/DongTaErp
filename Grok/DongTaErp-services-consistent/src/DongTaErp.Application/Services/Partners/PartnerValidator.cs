namespace DongTaErp.Application.Services.Partners;


public class CreatePartnerValidator : AbstractValidator<CreatePartnerDto>
{
    public CreatePartnerValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(40);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Phone)
            .MaximumLength(40);

        RuleFor(x => x.Email)
            .MaximumLength(160)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.TaxCode)
            .MaximumLength(40);

        RuleFor(x => x.Address)
            .MaximumLength(300);
    }
}

public class UpdatePartnerValidator : AbstractValidator<UpdatePartnerDto>
{
    public UpdatePartnerValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Code)
            .MaximumLength(40)
            .When(x => x.Code != null);

        RuleFor(x => x.Name)
            .MaximumLength(200)
            .When(x => x.Name != null);

        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue);

        RuleFor(x => x.Phone)
            .MaximumLength(40)
            .When(x => x.Phone != null);

        RuleFor(x => x.Email)
            .MaximumLength(160)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.TaxCode)
            .MaximumLength(40)
            .When(x => x.TaxCode != null);

        RuleFor(x => x.Address)
            .MaximumLength(300)
            .When(x => x.Address != null);
    }
}