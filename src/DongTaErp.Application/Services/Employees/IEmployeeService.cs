namespace DongTaErp.Application.Services.Employees;

public interface IEmployeeService : ICrudService<EmployeeDto, CreateEmployeeDto, UpdateEmployeeDto>
{
    Task<Result<PaginatedList<EmployeeDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
}
