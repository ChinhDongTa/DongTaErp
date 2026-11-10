
namespace DongTaErp.Application.Services.Departments;

public interface IDepartmentService
{
    Task<Result<DepartmentDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateDepartmentDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<DepartmentDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateDepartmentDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateDepartmentDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

}