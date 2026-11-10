namespace DongTaErp.Application.Services.Disbursements;

public interface IDisbursementService
{
    Task<Result<DisbursementDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateDisbursementDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<DisbursementDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateDisbursementDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateDisbursementDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<DisbursementDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result> PostAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
}
