namespace DongTaErp.Application.Services.PurchaseOrders;

public interface IPurchaseOrderService : ICrudService<PurchaseOrderDto, CreatePurchaseOrderDto, UpdatePurchaseOrderDto>
{
    Task<Result<PaginatedList<PurchaseOrderDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result> ConfirmAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
}
