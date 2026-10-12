namespace DongTaErp.Application.Services.JobPositions;

public interface IJobPositionService : ICrudService<JobPositionDto, CreateJobPositionDto, UpdateJobPositionDto>
{
    Task<Result<PaginatedList<JobPositionDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
}
