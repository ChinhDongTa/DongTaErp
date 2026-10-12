namespace DongTaErp.Application.Services.GoodsReceipts;

public class CreateGoodsReceiptValidator : AbstractValidator<CreateGoodsReceiptDto>
{
    public CreateGoodsReceiptValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.PurchaseOrderId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.SupplierRef).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(400);
        RuleFor(x => x.Lines).NotEmpty();
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.PurchaseOrderLineId).NotEmpty();
            line.RuleFor(x => x.Quantity).GreaterThan(0);
            line.RuleFor(x => x.Note).MaximumLength(300);
        });
    }
}

public class UpdateGoodsReceiptValidator : AbstractValidator<UpdateGoodsReceiptDto>
{
    public UpdateGoodsReceiptValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).MaximumLength(30).When(x => x.Code != null);
        RuleFor(x => x.SupplierRef).MaximumLength(100).When(x => x.SupplierRef != null);
        RuleFor(x => x.Notes).MaximumLength(400).When(x => x.Notes != null);
    }
}
