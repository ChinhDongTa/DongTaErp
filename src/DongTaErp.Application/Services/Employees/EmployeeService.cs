namespace DongTaErp.Application.Services.Employees;

public class EmployeeService
    : Base.GenericCrudService<Employee, EmployeeDto, CreateEmployeeDto, UpdateEmployeeDto>,
      IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(
        IUnitOfWork unitOfWork,
        ILogger<EmployeeService> logger,
        IValidator<CreateEmployeeDto> createValidator,
        IValidator<UpdateEmployeeDto> updateValidator)
        : base(unitOfWork.Employees, unitOfWork, logger, createValidator, updateValidator)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<EmployeeDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default)
    {
        try
        {
            var query = _unitOfWork.Employees.Query();

            if (!string.IsNullOrWhiteSpace(search.SearchTerm))
            {
                var term = $"%{search.SearchTerm.ToLower()}%";
                query = query.Where(e =>
                    EF.Functions.Like(e.Code.ToLower(), term) ||
                    EF.Functions.Like(e.FullName.ToLower(), term) ||
                    (e.Email != null && EF.Functions.Like(e.Email.ToLower(), term)) ||
                    (e.Phone != null && EF.Functions.Like(e.Phone.ToLower(), term)) ||
                    EF.Functions.Like(e.IdentityNumber.ToLower(), term));
            }

            query = string.IsNullOrWhiteSpace(search.SortBy)
                ? query.ApplySorting()
                : query.ApplySorting(search.SortBy, search.IsDescending);

            var page = await query.ToEmployeeDto()
                .PaginatedListAsync(search.PageNumber, search.PageSize, ct);

            return Result<PaginatedList<EmployeeDto>>.Success(page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching employees");
            return Result<PaginatedList<EmployeeDto>>.CriticalError("Internal server error occurred while searching employees.");
        }
    }

    protected override Expression<Func<Employee, EmployeeDto>> ToDto() => Extensions.ExpressionToDto();

    protected override Employee CreateEntity(CreateEmployeeDto dto) => new()
    {
        Code = dto.Code.Trim(),
        FullName = dto.FullName.Trim(),
        Email = dto.Email,
        Phone = dto.Phone,
        IdentityNumber = dto.IdentityNumber.Trim(),
        Address = dto.Address,
        BirthDate = dto.BirthDate,
        HireDate = dto.HireDate,
        ResignDate = dto.ResignDate,
        Gender = dto.Gender,
        Status = dto.Status,
        DepartmentId = dto.DepartmentId,
        PositionId = dto.PositionId,
        UserId = dto.UserId
    };

    protected override Expression<Func<Employee, UpdateEmployeeDto>> ToUpdateDto() => e => new UpdateEmployeeDto
    {
        Id = e.Id,
        Code = e.Code,
        FullName = e.FullName,
        Email = e.Email,
        Phone = e.Phone,
        IdentityNumber = e.IdentityNumber,
        Address = e.Address,
        BirthDate = e.BirthDate,
        HireDate = e.HireDate,
        ResignDate = e.ResignDate,
        Gender = e.Gender,
        Status = e.Status,
        DepartmentId = e.DepartmentId,
        PositionId = e.PositionId,
        UserId = e.UserId
    };

    protected override void UpdateEntity(Employee entity, UpdateEmployeeDto dto)
    {
        if (dto.Code.HasValueAndIsDifferentFrom(entity.Code))
            entity.Code = dto.Code!.Trim();

        if (dto.FullName.HasValueAndIsDifferentFrom(entity.FullName))
            entity.FullName = dto.FullName!.Trim();

        if (dto.Email.IsDifferentFrom(entity.Email))
            entity.Email = dto.Email;

        if (dto.Phone.IsDifferentFrom(entity.Phone))
            entity.Phone = dto.Phone;

        if (dto.IdentityNumber.HasValueAndIsDifferentFrom(entity.IdentityNumber))
            entity.IdentityNumber = dto.IdentityNumber!.Trim();

        if (dto.Address.IsDifferentFrom(entity.Address))
            entity.Address = dto.Address;

        if (dto.BirthDate.IsDifferentFrom(entity.BirthDate))
            entity.BirthDate = dto.BirthDate;

        if (dto.HireDate.HasValueAndIsDifferentFrom(entity.HireDate))
            entity.HireDate = dto.HireDate!.Value;

        if (dto.ResignDate.IsDifferentFrom(entity.ResignDate))
            entity.ResignDate = dto.ResignDate;

        if (dto.Gender.HasValueAndIsDifferentFrom(entity.Gender))
            entity.Gender = dto.Gender!.Value;

        if (dto.Status.HasValueAndIsDifferentFrom(entity.Status))
            entity.Status = dto.Status!.Value;

        if (dto.DepartmentId.HasValueAndIsDifferentFrom(entity.DepartmentId))
            entity.DepartmentId = dto.DepartmentId!.Value;

        if (dto.PositionId.HasValueAndIsDifferentFrom(entity.PositionId))
            entity.PositionId = dto.PositionId!.Value;

        if (dto.UserId.IsDifferentFrom(entity.UserId))
            entity.UserId = dto.UserId;
    }
}
