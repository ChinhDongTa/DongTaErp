namespace DongTaErp.Application.Services.Departments;

public class DepartmentService(
    IUnitOfWork unitOfWork,
    ILogger<DepartmentService> logger,
    IValidator<CreateDepartmentDto> createValidator,
    IValidator<UpdateDepartmentDto> updateValidator) : IDepartmentService
{
    public Task<Result<DepartmentDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetByIdAsync(unitOfWork.Departments, ToDto(), id, logger, ct);

    public Task<Result<PaginatedList<DepartmentDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => CrudOperations.GetAllAsync(unitOfWork.Departments, ToDto(), pageNumber, pageSize, logger, ct);

    public Task<Result<string>> CreateAsync(CreateDepartmentDto dto, CancellationToken ct = default)
        => CrudOperations.CreateAsync(unitOfWork.Departments, unitOfWork, createValidator, CreateEntity, dto, logger, ct);

    public Task<Result> UpdateAsync(Guid id, UpdateDepartmentDto dto, CancellationToken ct = default)
        => CrudOperations.UpdateAsync(unitOfWork.Departments, unitOfWork, updateValidator, UpdateEntity, id, dto, logger, ct);

    public Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.DeleteAsync(unitOfWork.Departments, unitOfWork, id, logger, ct);

    public Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.SoftDeleteAsync(unitOfWork.Departments, unitOfWork, id, logger, ct);

    public Task<Result<UpdateDepartmentDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetUpdateByIdAsync(unitOfWork.Departments, ToUpdateDto(), id, logger, ct);

    private static Expression<Func<Department, DepartmentDto>> ToDto() => d => new DepartmentDto
    {
        Id = d.Id,
        Code = d.Code,
        Name = d.Name,
        Note = d.Note,
        IsActive = d.IsActive,
        CreatedAt = d.Created
    };

    private static Department CreateEntity(CreateDepartmentDto dto) => new()
    {
        Code = dto.Code,
        Name = dto.Name,
        Note = dto.Note,
        IsActive = dto.IsActive
    };

    private static Expression<Func<Department, UpdateDepartmentDto>> ToUpdateDto() => d => new UpdateDepartmentDto
    {
        Id = d.Id,
        Code = d.Code,
        Name = d.Name,
        Note = d.Note,
        IsActive = d.IsActive
    };

    private static void UpdateEntity(Department department, UpdateDepartmentDto dto)
    {
        // Required fields
        if (dto.Code.HasValueAndIsDifferentFrom(department.Code))
        {
            department.Code = dto.Code!;
        }

        if (dto.Name.HasValueAndIsDifferentFrom(department.Name))
        {
            department.Name = dto.Name!;
        }

        // Optional fields
        if (dto.Note.IsDifferentFrom(department.Note))
        {
            department.Note = dto.Note;
        }

        if (dto.IsActive.IsDifferentFrom(department.IsActive))
        {
            department.IsActive = dto.IsActive;
        }
    }
}