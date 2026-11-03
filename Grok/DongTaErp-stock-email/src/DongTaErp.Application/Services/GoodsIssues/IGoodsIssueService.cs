namespace DongTaErp.Application.Services.GoodsIssues;

public interface IGoodsIssueService : ICrudService<GoodsIssueDto, CreateGoodsIssueDto, UpdateGoodsIssueDto>
{
    Task<Result<PaginatedList<GoodsIssueDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result<string>> CreateAndPostAsync(CreateGoodsIssueDto dto, CancellationToken ct = default);
    Task<Result> PostAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
    Task<Result<List<DocumentLineLookupDto>>> GetGoodsIssueLinesAsync(Guid salesOrderId, Guid? warehouseId = null, CancellationToken ct = default);
}
