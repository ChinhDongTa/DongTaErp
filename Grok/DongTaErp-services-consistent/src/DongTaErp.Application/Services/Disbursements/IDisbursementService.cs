namespace DongTaErp.Application.Services.Disbursements;

public interface IDisbursementService : ICrudService<DisbursementDto, CreateDisbursementDto, UpdateDisbursementDto>
{
    Task<Result<PaginatedList<DisbursementDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result> PostAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
}
