namespace DongTaErp.Application.Services.GoodsIssues;

public class CreateGoodsIssueValidator : AbstractValidator<CreateGoodsIssueDto>
{
    public CreateGoodsIssueValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.SalesOrderId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(400);
        RuleFor(x => x.Lines).NotEmpty();
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.SalesOrderLineId).NotEmpty();
            line.RuleFor(x => x.Quantity).GreaterThan(0);
            line.RuleFor(x => x.Note).MaximumLength(300);
        });
    }
}

public class UpdateGoodsIssueValidator : AbstractValidator<UpdateGoodsIssueDto>
{
    public UpdateGoodsIssueValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).MaximumLength(30).When(x => x.Code != null);
        RuleFor(x => x.Notes).MaximumLength(400).When(x => x.Notes != null);
    }
}
