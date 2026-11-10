namespace DongTaErp.Application.Services.GoodsReceipts;

public interface IGoodsReceiptService
{
    Task<Result<GoodsReceiptDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateGoodsReceiptDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<GoodsReceiptDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateGoodsReceiptDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateGoodsReceiptDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<GoodsReceiptDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result> PostAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
    Task<Result<List<DocumentLineLookupDto>>> GetGoodsReceiptLinesAsync(Guid purchaseOrderId, CancellationToken ct = default);
}
