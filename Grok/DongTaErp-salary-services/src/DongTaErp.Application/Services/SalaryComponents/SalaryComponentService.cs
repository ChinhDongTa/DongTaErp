namespace DongTaErp.Application.Services.SalaryComponents;

public class SalaryComponentService
    : Base.GenericCrudService<SalaryComponent, SalaryComponentDto, CreateSalaryComponentDto, UpdateSalaryComponentDto>,
      ISalaryComponentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SalaryComponentService> _logger;

    public SalaryComponentService(
        IUnitOfWork unitOfWork,
        ILogger<SalaryComponentService> logger,
        IValidator<CreateSalaryComponentDto> createValidator,
        IValidator<UpdateSalaryComponentDto> updateValidator)
        : base(unitOfWork.SalaryComponents, unitOfWork, logger, createValidator, updateValidator)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<SalaryComponentDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default)
    {
        try
        {
            var query = _unitOfWork.SalaryComponents.Query();
            if (!string.IsNullOrWhiteSpace(search.SearchTerm))
            {
                var term = $"%{search.SearchTerm.ToLower()}%";
                query = query.Where(x => EF.Functions.Like(x.Code.ToLower(), term) || EF.Functions.Like(x.Name.ToLower(), term));
            }
            query = string.IsNullOrWhiteSpace(search.SortBy) ? query.ApplySorting() : query.ApplySorting(search.SortBy, search.IsDescending);
            var page = await query.ToSalaryComponentDto().PaginatedListAsync(search.PageNumber, search.PageSize, ct);
            return Result<PaginatedList<SalaryComponentDto>>.Success(page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching salary components");
            return Result<PaginatedList<SalaryComponentDto>>.CriticalError("Internal server error occurred while searching salary components.");
        }
    }

    public async Task<Result<List<SalaryComponentDto>>> GetActiveAsync(bool? isFixed = null, CancellationToken ct = default)
    {
        var query = _unitOfWork.SalaryComponents.Query().Where(x => x.IsActive);
        if (isFixed is not null)
            query = query.Where(x => x.IsFixed == isFixed);
        var items = await query.OrderBy(x => x.Code).ToSalaryComponentDto().ToListAsync(ct);
        return Result<List<SalaryComponentDto>>.Success(items);
    }

    public override async Task<Result<string>> CreateAsync(CreateSalaryComponentDto dto, CancellationToken ct = default)
    {
        var code = dto.Code.Trim();
        if (await _unitOfWork.SalaryComponents.Query().AnyAsync(x => x.Code == code, ct))
            return Result<string>.Error($"Mã khoản lương {code} đã tồn tại.");
        dto.Code = code;
        dto.Name = dto.Name.Trim();
        return await base.CreateAsync(dto, ct);
    }

    protected override Expression<Func<SalaryComponent, SalaryComponentDto>> ToDto() => Extensions.ExpressionToDto();

    protected override SalaryComponent CreateEntity(CreateSalaryComponentDto dto) => new()
    {
        Code = dto.Code.Trim(),
        Name = dto.Name.Trim(),
        Type = dto.Type,
        IsFixed = dto.IsFixed,
        IsTaxable = dto.IsTaxable,
        IsActive = dto.IsActive,
        Note = dto.Note
    };

    protected override Expression<Func<SalaryComponent, UpdateSalaryComponentDto>> ToUpdateDto() => x => new UpdateSalaryComponentDto
    {
        Id = x.Id,
        Code = x.Code,
        Name = x.Name,
        Type = x.Type,
        IsFixed = x.IsFixed,
        IsTaxable = x.IsTaxable,
        IsActive = x.IsActive,
        Note = x.Note
    };

    protected override void UpdateEntity(SalaryComponent entity, UpdateSalaryComponentDto dto)
    {
        if (dto.Code.HasValueAndIsDifferentFrom(entity.Code))
            entity.Code = dto.Code!.Trim();
        if (dto.Name.HasValueAndIsDifferentFrom(entity.Name))
            entity.Name = dto.Name!.Trim();
        if (dto.Type.HasValueAndIsDifferentFrom(entity.Type))
            entity.Type = dto.Type!.Value;
        if (dto.IsFixed.HasValueAndIsDifferentFrom(entity.IsFixed))
            entity.IsFixed = dto.IsFixed!.Value;
        if (dto.IsTaxable.HasValueAndIsDifferentFrom(entity.IsTaxable))
            entity.IsTaxable = dto.IsTaxable!.Value;
        if (dto.IsActive.HasValueAndIsDifferentFrom(entity.IsActive))
            entity.IsActive = dto.IsActive!.Value;
        if (dto.Note.IsDifferentFrom(entity.Note))
            entity.Note = dto.Note;
    }
}
