namespace DongTaErp.Application.Services.PurchaseOrders;

public interface IPurchaseOrderService
{
    Task<Result<PurchaseOrderDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdatePurchaseOrderDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<PurchaseOrderDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreatePurchaseOrderDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdatePurchaseOrderDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<PurchaseOrderDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result> ConfirmAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
}
