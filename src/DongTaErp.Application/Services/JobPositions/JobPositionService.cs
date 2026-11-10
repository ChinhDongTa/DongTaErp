namespace DongTaErp.Application.Services.JobPositions;

public class JobPositionService(
    IUnitOfWork unitOfWork,
    ILogger<JobPositionService> logger,
    IValidator<CreateJobPositionDto> createValidator,
    IValidator<UpdateJobPositionDto> updateValidator) : IJobPositionService
{
    public Task<Result<JobPositionDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetByIdAsync(unitOfWork.JobPositions, ToDto(), id, logger, ct);

    public Task<Result<PaginatedList<JobPositionDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => CrudOperations.GetAllAsync(unitOfWork.JobPositions, ToDto(), pageNumber, pageSize, logger, ct);

    public Task<Result<string>> CreateAsync(CreateJobPositionDto dto, CancellationToken ct = default)
        => CrudOperations.CreateAsync(unitOfWork.JobPositions, unitOfWork, createValidator, CreateEntity, dto, logger, ct);

    public Task<Result> UpdateAsync(Guid id, UpdateJobPositionDto dto, CancellationToken ct = default)
        => CrudOperations.UpdateAsync(unitOfWork.JobPositions, unitOfWork, updateValidator, UpdateEntity, id, dto, logger, ct);

    public Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.DeleteAsync(unitOfWork.JobPositions, unitOfWork, id, logger, ct);

    public Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.SoftDeleteAsync(unitOfWork.JobPositions, unitOfWork, id, logger, ct);

    public Task<Result<UpdateJobPositionDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetUpdateByIdAsync(unitOfWork.JobPositions, ToUpdateDto(), id, logger, ct);


    public async Task<Result<PaginatedList<JobPositionDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default)
    {
        try
        {
            var query = unitOfWork.JobPositions.Query();

            if (!string.IsNullOrWhiteSpace(search.SearchTerm))
            {
                var term = $"%{search.SearchTerm.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.Code.ToLower(), term) ||
                    EF.Functions.Like(x.Name.ToLower(), term));
            }

            query = string.IsNullOrWhiteSpace(search.SortBy)
                ? query.ApplySorting()
                : query.ApplySorting(search.SortBy, search.IsDescending);

            var page = await query.ToJobPositionDto()
                .PaginatedListAsync(search.PageNumber, search.PageSize, ct);

            return Result<PaginatedList<JobPositionDto>>.Success(page);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching job positions");
            return Result<PaginatedList<JobPositionDto>>.CriticalError("Internal server error occurred while searching job positions.");
        }
    }

    private static Expression<Func<JobPosition, JobPositionDto>> ToDto() => Extensions.ExpressionToDto();

    private static JobPosition CreateEntity(CreateJobPositionDto dto) => new()
    {
        Code = dto.Code.Trim(),
        Name = dto.Name.Trim(),
        IsActive = dto.IsActive,
        IsDirector = dto.IsDirector
    };

    private static Expression<Func<JobPosition, UpdateJobPositionDto>> ToUpdateDto() => x => new UpdateJobPositionDto
    {
        Id = x.Id,
        Code = x.Code,
        Name = x.Name,
        IsActive = x.IsActive,
        IsDirector = x.IsDirector
    };

    private static void UpdateEntity(JobPosition entity, UpdateJobPositionDto dto)
    {
        if (dto.Code.HasValueAndIsDifferentFrom(entity.Code))
            entity.Code = dto.Code!.Trim();

        if (dto.Name.HasValueAndIsDifferentFrom(entity.Name))
            entity.Name = dto.Name!.Trim();

        if (dto.IsActive.HasValueAndIsDifferentFrom(entity.IsActive))
            entity.IsActive = dto.IsActive!.Value;
        if (dto.IsDirector.HasValueAndIsDifferentFrom(entity.IsDirector))
            entity.IsDirector = dto.IsDirector!.Value;
    }
}
