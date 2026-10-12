namespace DongTaErp.Application.Services.JobPositions;

public class JobPositionService
    : Base.GenericCrudService<JobPosition, JobPositionDto, CreateJobPositionDto, UpdateJobPositionDto>,
      IJobPositionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<JobPositionService> _logger;

    public JobPositionService(
        IUnitOfWork unitOfWork,
        ILogger<JobPositionService> logger,
        IValidator<CreateJobPositionDto> createValidator,
        IValidator<UpdateJobPositionDto> updateValidator)
        : base(unitOfWork.JobPositions, unitOfWork, logger, createValidator, updateValidator)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<JobPositionDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default)
    {
        try
        {
            var query = _unitOfWork.JobPositions.Query();

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
            _logger.LogError(ex, "Error searching job positions");
            return Result<PaginatedList<JobPositionDto>>.CriticalError("Internal server error occurred while searching job positions.");
        }
    }

    protected override Expression<Func<JobPosition, JobPositionDto>> ToDto() => Extensions.ExpressionToDto();

    protected override JobPosition CreateEntity(CreateJobPositionDto dto) => new()
    {
        Code = dto.Code.Trim(),
        Name = dto.Name.Trim(),
        IsActive = dto.IsActive
    };

    protected override Expression<Func<JobPosition, UpdateJobPositionDto>> ToUpdateDto() => x => new UpdateJobPositionDto
    {
        Id = x.Id,
        Code = x.Code,
        Name = x.Name,
        IsActive = x.IsActive
    };

    protected override void UpdateEntity(JobPosition entity, UpdateJobPositionDto dto)
    {
        if (dto.Code.HasValueAndIsDifferentFrom(entity.Code))
            entity.Code = dto.Code!.Trim();

        if (dto.Name.HasValueAndIsDifferentFrom(entity.Name))
            entity.Name = dto.Name!.Trim();

        if (dto.IsActive.HasValueAndIsDifferentFrom(entity.IsActive))
            entity.IsActive = dto.IsActive!.Value;
    }
}
