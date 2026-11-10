namespace DongTaErp.Application.Services.JobPositions;

public interface IJobPositionService
{
    Task<Result<JobPositionDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateJobPositionDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<JobPositionDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateJobPositionDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateJobPositionDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<JobPositionDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
}
