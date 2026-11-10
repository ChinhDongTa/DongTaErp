namespace DongTaErp.Application.Services.Employees;

public interface IEmployeeService
{
    Task<Result<EmployeeDto?>> GetEmployeeByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<EmployeeDto>>> GetAllEmployeesAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<PaginatedList<EmployeeDto>>> SearchAsync(string? searchTerm, int pageNumber = 1, int pageSize = 20, string? sortBy = null, bool isDescending = false, CancellationToken ct = default);
    Task<Result<EmployeeDto>> CreateAsync(CreateEmployeeDto dto, CancellationToken ct = default);
    Task<Result<EmployeeDto>> UpdateAsync(Guid id, UpdateEmployeeDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}