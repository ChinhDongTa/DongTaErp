namespace DongTaErp.Application.Services.Products;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty()
            .MaximumLength(40);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Unit)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.SalePrice)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.MinStock)
            .GreaterThanOrEqualTo(0);
    }
}

public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Sku)
            .MaximumLength(40)
            .When(x => x.Sku != null);

        RuleFor(x => x.Name)
            .MaximumLength(200)
            .When(x => x.Name != null);

        RuleFor(x => x.Unit)
            .MaximumLength(20)
            .When(x => x.Unit != null);

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.CostPrice.HasValue);

        RuleFor(x => x.SalePrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.SalePrice.HasValue);

        RuleFor(x => x.MinStock)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinStock.HasValue);
    }
}