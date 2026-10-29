namespace DongTaErp.Application.Services.EmployeeCompensations;

public class EmployeeCompensationService
    : Base.GenericCrudService<EmployeeCompensation, EmployeeCompensationDto, CreateEmployeeCompensationDto, UpdateEmployeeCompensationDto>,
      IEmployeeCompensationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeCompensationService> _logger;

    public EmployeeCompensationService(
        IUnitOfWork unitOfWork,
        ILogger<EmployeeCompensationService> logger,
        IValidator<CreateEmployeeCompensationDto> createValidator,
        IValidator<UpdateEmployeeCompensationDto> updateValidator)
        : base(unitOfWork.EmployeeCompensations, unitOfWork, logger, createValidator, updateValidator)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<EmployeeCompensationDto>>> SearchAsync(SimpleSearch search, Guid? employeeId = null, CancellationToken ct = default)
    {
        try
        {
            var query = _unitOfWork.EmployeeCompensations.Query();
            if (employeeId is not null)
                query = query.Where(x => x.EmployeeId == employeeId);
            if (!string.IsNullOrWhiteSpace(search.SearchTerm))
            {
                var term = $"%{search.SearchTerm.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.Employee.FullName.ToLower(), term) ||
                    EF.Functions.Like(x.Employee.Code.ToLower(), term) ||
                    EF.Functions.Like(x.SalaryComponent.Code.ToLower(), term) ||
                    EF.Functions.Like(x.SalaryComponent.Name.ToLower(), term));
            }
            query = string.IsNullOrWhiteSpace(search.SortBy) ? query.ApplySorting() : query.ApplySorting(search.SortBy, search.IsDescending);
            var page = await query.ToEmployeeCompensationDto().PaginatedListAsync(search.PageNumber, search.PageSize, ct);
            return Result<PaginatedList<EmployeeCompensationDto>>.Success(page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching employee compensations");
            return Result<PaginatedList<EmployeeCompensationDto>>.CriticalError("Internal server error occurred while searching employee compensations.");
        }
    }

    public async Task<Result<List<EmployeeCompensationDto>>> GetActiveAsync(Guid employeeId, DateOnly? asOf = null, CancellationToken ct = default)
    {
        var date = asOf ?? DateOnly.FromDateTime(DateTime.Today);
        var items = await _unitOfWork.EmployeeCompensations.Query()
            .Where(x => x.EmployeeId == employeeId && x.EffectiveFrom <= date && (x.EffectiveTo == null || x.EffectiveTo >= date))
            .OrderBy(x => x.SalaryComponent.Code)
            .ToEmployeeCompensationDto()
            .ToListAsync(ct);
        return Result<List<EmployeeCompensationDto>>.Success(items);
    }

    public override async Task<Result<string>> CreateAsync(CreateEmployeeCompensationDto dto, CancellationToken ct = default)
    {
        var error = await ValidateOverlapAsync(dto.EmployeeId, dto.SalaryComponentId, dto.EffectiveFrom, dto.EffectiveTo, null, ct);
        if (error is not null)
            return Result<string>.Error(error);
        return await base.CreateAsync(dto, ct);
    }

    public override async Task<Result> UpdateAsync(Guid id, UpdateEmployeeCompensationDto dto, CancellationToken ct = default)
    {
        var current = await _unitOfWork.EmployeeCompensations.Query().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (current is null)
            return Result.NotFound("Không tìm thấy khoản lương hợp đồng.");
        var employeeId = dto.EmployeeId ?? current.EmployeeId;
        var componentId = dto.SalaryComponentId ?? current.SalaryComponentId;
        var from = dto.EffectiveFrom ?? current.EffectiveFrom;
        var to = dto.EffectiveTo ?? current.EffectiveTo;
        var error = await ValidateOverlapAsync(employeeId, componentId, from, to, id, ct);
        if (error is not null)
            return Result.Error(error);
        return await base.UpdateAsync(id, dto, ct);
    }

    protected override Expression<Func<EmployeeCompensation, EmployeeCompensationDto>> ToDto() => Extensions.ExpressionToDto();

    protected override EmployeeCompensation CreateEntity(CreateEmployeeCompensationDto dto) => new()
    {
        EmployeeId = dto.EmployeeId,
        SalaryComponentId = dto.SalaryComponentId,
        Amount = dto.Amount,
        EffectiveFrom = dto.EffectiveFrom,
        EffectiveTo = dto.EffectiveTo,
        Note = dto.Note
    };

    protected override Expression<Func<EmployeeCompensation, UpdateEmployeeCompensationDto>> ToUpdateDto() => x => new UpdateEmployeeCompensationDto
    {
        Id = x.Id,
        EmployeeId = x.EmployeeId,
        SalaryComponentId = x.SalaryComponentId,
        Amount = x.Amount,
        EffectiveFrom = x.EffectiveFrom,
        EffectiveTo = x.EffectiveTo,
        Note = x.Note
    };

    protected override void UpdateEntity(EmployeeCompensation entity, UpdateEmployeeCompensationDto dto)
    {
        if (dto.EmployeeId.HasValueAndIsDifferentFrom(entity.EmployeeId))
            entity.EmployeeId = dto.EmployeeId!.Value;
        if (dto.SalaryComponentId.HasValueAndIsDifferentFrom(entity.SalaryComponentId))
            entity.SalaryComponentId = dto.SalaryComponentId!.Value;
        if (dto.Amount.HasValueAndIsDifferentFrom(entity.Amount))
            entity.Amount = dto.Amount!.Value;
        if (dto.EffectiveFrom.HasValueAndIsDifferentFrom(entity.EffectiveFrom))
            entity.EffectiveFrom = dto.EffectiveFrom!.Value;
        if (dto.EffectiveTo.IsDifferentFrom(entity.EffectiveTo))
            entity.EffectiveTo = dto.EffectiveTo;
        if (dto.Note.IsDifferentFrom(entity.Note))
            entity.Note = dto.Note;
    }

    private async Task<string?> ValidateOverlapAsync(Guid employeeId, Guid componentId, DateOnly from, DateOnly? to, Guid? ignoreId, CancellationToken ct)
    {
        if (to is not null && to < from)
            return "Ngày hết hiệu lực phải sau ngày bắt đầu.";
        var component = await _unitOfWork.SalaryComponents.Query().FirstOrDefaultAsync(x => x.Id == componentId, ct);
        if (component is null)
            return "Khoản lương không tồn tại.";
        if (!component.IsFixed)
            return "Chỉ khoản cố định mới được gán vào hợp đồng lương.";
        var end = to ?? DateOnly.MaxValue;
        var overlapped = await _unitOfWork.EmployeeCompensations.Query().AnyAsync(x =>
            x.Id != ignoreId &&
            x.EmployeeId == employeeId &&
            x.SalaryComponentId == componentId &&
            x.EffectiveFrom <= end &&
            (x.EffectiveTo == null || x.EffectiveTo >= from), ct);
        return overlapped ? "Khoản này đã có hiệu lực giao với khoảng thời gian đã chọn." : null;
    }
}
