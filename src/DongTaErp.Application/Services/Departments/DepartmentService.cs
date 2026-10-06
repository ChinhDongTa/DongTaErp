namespace DongTaErp.Application.Services.Departments;

public class DepartmentService : Base.GenericCrudService<Department, DepartmentDto, CreateDepartmentDto, UpdateDepartmentDto>, IDepartmentService
{
    public DepartmentService(IUnitOfWork unitOfWork, ILogger<DepartmentService> logger) : base(unitOfWork.Departments, unitOfWork, logger)
    {
    }

    protected override Expression<Func<Department, DepartmentDto>> ToDto() => d => new DepartmentDto
    {
        Id = d.Id,
        Code = d.Code,
        Name = d.Name,
        Note = d.Note,
        IsActive = d.IsActive,
        CreatedAt = d.Created
    };

    protected override Department CreateEntity(CreateDepartmentDto dto) => new()
    {
        Code = dto.Code,
        Name = dto.Name,
        Note = dto.Note,
        IsActive = dto.IsActive
    };

    protected override Expression<Func<Department, UpdateDepartmentDto>> ToUpdateDto() => d => new UpdateDepartmentDto
    {
        Id = d.Id,
        Code = d.Code,
        Name = d.Name,
        Note = d.Note,
        IsActive = d.IsActive
    };

    protected override void UpdateEntity(Department department, UpdateDepartmentDto dto)
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
            department.IsActive = dto.IsActive!.Value;
        }
    }
}