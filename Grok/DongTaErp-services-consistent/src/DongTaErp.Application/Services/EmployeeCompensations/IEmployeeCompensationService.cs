namespace DongTaErp.Application.Services.EmployeeCompensations;

public interface IEmployeeCompensationService : ICrudService<EmployeeCompensationDto, CreateEmployeeCompensationDto, UpdateEmployeeCompensationDto>
{
    Task<Result<PaginatedList<EmployeeCompensationDto>>> SearchAsync(SimpleSearch search, Guid? employeeId = null, CancellationToken ct = default);
    Task<Result<List<EmployeeCompensationDto>>> GetActiveAsync(Guid employeeId, DateOnly? asOf = null, CancellationToken ct = default);
}
