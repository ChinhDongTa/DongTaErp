namespace DongTaErp.Application.Services.InvantoryTxn;

public class CreateInventoryTxnValidator : AbstractValidator<CreateInventoryTxnDto>
{
    public CreateInventoryTxnValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.WarehouseId)
            .NotEmpty();

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.QtyChange)
            .NotEqual(0);

        RuleFor(x => x.Reference)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(x => x.Note)
            .MaximumLength(300);
    }
}

public class UpdateInventoryTxnValidator : AbstractValidator<UpdateInventoryTxnDto>
{
    public UpdateInventoryTxnValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .When(x => x.ProductId.HasValue);

        RuleFor(x => x.WarehouseId)
            .NotEmpty()
            .When(x => x.WarehouseId.HasValue);

        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue);

        RuleFor(x => x.QtyChange)
            .NotEqual(0)
            .When(x => x.QtyChange.HasValue);

        RuleFor(x => x.Reference)
            .MaximumLength(80)
            .When(x => x.Reference != null);

        RuleFor(x => x.Note)
            .MaximumLength(300)
            .When(x => x.Note != null);
    }
}