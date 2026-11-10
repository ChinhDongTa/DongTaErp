namespace DongTaErp.Application.Services.Warehouses;

public class CreateWarehouseValidator : AbstractValidator<CreateWarehouseDto>
{
    public CreateWarehouseValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(160);

        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .MaximumLength(2);

        RuleFor(x => x.City)
            .MaximumLength(80);

        RuleFor(x => x.Address)
            .MaximumLength(300);

        RuleFor(x => x.TimeZone)
            .MaximumLength(64);
    }
}
public class UpdateWarehouseValidator : AbstractValidator<UpdateWarehouseDto>
{
    public UpdateWarehouseValidator()
    {
        RuleFor(x => x.Code)
            .MaximumLength(30)
            .When(x => x.Code != null);

        RuleFor(x => x.Name)
            .MaximumLength(160)
            .When(x => x.Name != null);

        RuleFor(x => x.CountryCode)
            .MaximumLength(2)
            .When(x => x.CountryCode != null);

        RuleFor(x => x.City)
            .MaximumLength(80)
            .When(x => x.City != null);

        RuleFor(x => x.Address)
            .MaximumLength(300)
            .When(x => x.Address != null);

        RuleFor(x => x.TimeZone)
            .MaximumLength(64)
            .When(x => x.TimeZone != null);
    }
}