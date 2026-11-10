namespace DongTaErp.Application.Services.EmployeeCompensations;

public interface IEmployeeCompensationService
{
    Task<Result<EmployeeCompensationDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateEmployeeCompensationDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<EmployeeCompensationDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateEmployeeCompensationDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateEmployeeCompensationDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<EmployeeCompensationDto>>> SearchAsync(SimpleSearch search, Guid? employeeId = null, CancellationToken ct = default);
    Task<Result<List<EmployeeCompensationDto>>> GetActiveAsync(Guid employeeId, DateOnly? asOf = null, CancellationToken ct = default);
}
