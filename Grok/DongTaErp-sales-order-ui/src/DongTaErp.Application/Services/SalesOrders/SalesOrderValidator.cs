namespace DongTaErp.Application.Services.SalesOrders;

public class CreateSalesOrderValidator : AbstractValidator<CreateSalesOrderDto>
{
    public CreateSalesOrderValidator()
    {
        RuleFor(x => x.Number).NotEmpty().MaximumLength(30);
        RuleFor(x => x.PartnerId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.TaxRate).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1);
        RuleFor(x => x.Notes).MaximumLength(400);
        RuleFor(x => x.Lines).NotEmpty();
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.ProductId).NotEmpty();
            line.RuleFor(x => x.Qty).GreaterThan(0);
            line.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        });
    }
}

public class UpdateSalesOrderValidator : AbstractValidator<UpdateSalesOrderDto>
{
    public UpdateSalesOrderValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Number).MaximumLength(30).When(x => x.Number != null);
        RuleFor(x => x.TaxRate).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1).When(x => x.TaxRate != null);
        RuleFor(x => x.Notes).MaximumLength(400).When(x => x.Notes != null);
    }
}
