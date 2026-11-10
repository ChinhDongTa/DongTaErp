namespace DongTaErp.Application.Services.GoodsIssues;

public interface IGoodsIssueService
{
    Task<Result<GoodsIssueDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateGoodsIssueDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<GoodsIssueDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateGoodsIssueDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateGoodsIssueDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<GoodsIssueDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result<string>> CreateAndPostAsync(CreateGoodsIssueDto dto, CancellationToken ct = default);
    Task<Result> PostAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
    Task<Result<List<DocumentLineLookupDto>>> GetGoodsIssueLinesAsync(Guid salesOrderId, Guid? warehouseId = null, CancellationToken ct = default);
}
