namespace DongTaErp.Application.Services.GoodsReceipts;

public interface IGoodsReceiptService : ICrudService<GoodsReceiptDto, CreateGoodsReceiptDto, UpdateGoodsReceiptDto>
{
    Task<Result<PaginatedList<GoodsReceiptDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result> PostAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
}
